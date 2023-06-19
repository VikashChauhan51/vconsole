using System.Reflection;

namespace VConsole;

public sealed class Parser : IDisposable
{
    private readonly Dictionary<string, Type> commands = new Dictionary<string, Type>();
    private readonly IDependencyResolver dependencyResolver;
    private bool disposed;
    private readonly ParserSettings settings;
    private static readonly Lazy<Parser> DefaultParser = new Lazy<Parser>(
        () => new Parser());

    public Parser()
    {
        settings = new ParserSettings();
    }
    public Parser(IDependencyResolver dependencyResolver)
    {
        this.dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));

        settings = new ParserSettings();
    }

    public Parser(IDependencyResolver dependencyResolver, Action<ParserSettings> configuration)
    {
        this.dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        settings = new ParserSettings();
        configuration(settings);
    }

    public Parser(IDependencyResolver dependencyResolver, ParserSettings settings)
    {
        this.dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
        this.settings = settings;
    }


    public static Parser Default
    {
        get { return DefaultParser.Value; }
    }

    public ParserSettings Settings
    {
        get { return settings; }
    }

    public Parser RegisterCommand<TCommandLineCommand>()
            where TCommandLineCommand : ICommand
    {
        RegisterCommand(typeof(TCommandLineCommand));
        return this;
    }
    public Parser RegisterCommandsFromAssembly(Assembly assembly)
    {
        var commandTypes = assembly.ExportedTypes.ToDictionary(t => t, t => t.GetTypeInfo().GetCustomAttribute<VerbAttribute>());
        foreach (var pair in commandTypes.Where(p => !string.IsNullOrEmpty(p.Value?.Name) && p.Key.GetTypeInfo().IsClass && !p.Key.GetTypeInfo().IsAbstract))
            if (pair.Value != null)
                RegisterCommand(pair.Value.Name, pair.Key);

        return this;
    }
    public Parser RegisterCommand(Type commandType)
    {
        if (commandType is null)
            throw new ArgumentNullException(nameof(commandType));

        var commandAttribute = commandType.GetTypeInfo().GetCustomAttribute<VerbAttribute>();
        if (commandAttribute == null)
            throw new InvalidOperationException("The command class is missing the CommandAttribute attribute.");

        RegisterCommand(commandAttribute.Name, commandType);

        return this;
    }


    public void ParseArguments(string[] args)
    {
        if (args == null) throw new ArgumentNullException(nameof(args));
        if (commands.Count == 0) throw new ArgumentOutOfRangeException(nameof(commands));

        var types = commands.Values.ToArray();
        var verbs = Verb.SelectFromTypes(types);
        var commandName = args.First();

        var verbUsed = verbs.FirstOrDefault(vt =>
                settings.NameComparer.Equals(vt.Item1.Name, commandName)
                || vt.Item1.Aliases.Any(alias => settings.NameComparer.Equals(alias, commandName))
        );

        if (verbUsed == default)
        {
            throw new InvalidOperationException($"Could not retrieve command from arguments {string.Join(", ", args)}");
        }

        var parameters = args.Skip(1).ToArray();
        var commandType = commands[verbUsed.Item1.Name];
        var command = CreateCommand(commandType);

        foreach (var property in commandType.GetRuntimeProperties())
        {
            var argumentAttribute = property.GetCustomAttribute<OptionAttribute>();
            if (argumentAttribute != null)
            {
                var value = argumentAttribute
                    .GetValue(property, settings.Separator, parameters, settings.NameComparer, settings.ParsingCulture) ??
                    argumentAttribute.Default;

                if (argumentAttribute.Required && value == null)
                {
                    throw new ArgumentNullException($"Value can't be null for property {property.Name}");
                }

                if (value != null)
                {
                    property.SetValue(command, value);
                }
                    
            }
        }

        command.Execute();

    }

    
    public Parser ClearCommands()
    {
        commands.Clear();
        return this;
    }
    ~Parser()
    {
        Dispose(false);
    }
    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    private ICommand CreateCommand(Type commandType)
    {
        var constructors = commandType.GetTypeInfo().DeclaredConstructors;

        if (constructors.Any())
        {
            var constructor = constructors.First(c => !c.IsStatic);

            if (constructor.GetParameters().Length > 0 && dependencyResolver == null)
                throw new InvalidOperationException("No dependency resolver available to create a command without default constructor.");

            var parameters = constructor.GetParameters()
                .Select(param => dependencyResolver.GetService(param.ParameterType))
                .ToArray();

            return (ICommand)constructor.Invoke(parameters);
        }

        return (ICommand)Activator.CreateInstance(commandType);
    }

    private void RegisterCommand(string name, Type commandType)
    {
        if (commandType is null)
            throw new ArgumentNullException(nameof(commandType));

        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        if (commands.ContainsKey(name))
            throw new InvalidOperationException("The command '" + name + "' has already been added.");

        commands.Add(name, commandType);

    }
    private void Dispose(bool disposing)
    {
        if (disposed) return;

        if (disposing)
        {
            if (settings != null)
                settings.Dispose();

            disposed = true;
        }
    }

}

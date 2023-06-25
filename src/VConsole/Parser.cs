using System.Reflection;
using VConsole.Core;

namespace VConsole;

public sealed class Parser : IDisposable
{
    private readonly Dictionary<string, Type> commands = new Dictionary<string, Type>();
    private readonly IDependencyResolver? dependencyResolver;
    private bool disposed;
    private readonly ParserSettings settings;
    private readonly IConsole console;
    private static readonly Lazy<Parser> DefaultParser = new Lazy<Parser>(
        () => new Parser());

    public Parser()
    {
        settings = new ParserSettings();
        console = GetConsoleHost();
        AddDefaultCommands();
    }

    public Parser(ParserSettings settings)
    {
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        console = GetConsoleHost();
        AddDefaultCommands();
    }
    public Parser(IDependencyResolver dependencyResolver)
    {
        this.dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));

        settings = new ParserSettings();
        console = GetConsoleHost();
        AddDefaultCommands();
    }

    public Parser(IDependencyResolver dependencyResolver, Action<ParserSettings> configuration)
    {
        this.dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        console = GetConsoleHost();
        AddDefaultCommands();
        settings = new ParserSettings();
        configuration(settings);

    }

    public Parser(IDependencyResolver dependencyResolver, ParserSettings settings)
    {
        this.dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
        this.settings = settings;
        console = GetConsoleHost();
        AddDefaultCommands();
    }


    public static Parser Default
    {
        get { return DefaultParser.Value; }
    }

    public ParserSettings Settings
    {
        get { return settings; }
    }
    public IReadOnlyDictionary<string, Type> Commands => commands;
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


    public Task ParseArguments(string[] args)
    {
        if (commands.Count == 0) throw new ArgumentOutOfRangeException(nameof(commands));
        if (args == null) throw new ArgumentNullException(nameof(args));
        if (!args.Any() && !settings.InteractiveMode) throw new ArgumentException(nameof(args));

        var types = commands.Values.ToArray();
        var verbs = Verb.SelectFromTypes(types);

        if (!args.Any())
        {
            var input = ReadCommandNameInteractive();
            args = input.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToArray();
        }

        var commandName = args.First();

        var verbName = GetCommandVerbName(verbs, commandName);
        if (string.IsNullOrEmpty(verbName))
        {
            throw new InvalidOperationException($"Could not retrieve command from arguments {string.Join(", ", args)}");
        }

        var parameters = args.Skip(1).ToArray();
        var commandType = commands[verbName];
        var command = CreateCommand(commandType);

        foreach (var property in commandType.GetRuntimeProperties())
        {
            var argumentAttribute = property.GetCustomAttribute<OptionAttribute>();
            if (argumentAttribute != null)
            {
                var value = parameters.Any() ? (GetValue(property, argumentAttribute, parameters) ??
                    argumentAttribute.Default) : null;

                if (argumentAttribute.Required && value == null && !settings.InteractiveMode)
                {
                    throw new ArgumentNullException($"Value can't be null for property {property.Name}");
                }

                if (argumentAttribute.Required && value == null && settings.InteractiveMode)
                {
                    var name = !string.IsNullOrWhiteSpace(argumentAttribute.LongName) ?
                        $"--{argumentAttribute.LongName}"
                        : !string.IsNullOrWhiteSpace(argumentAttribute.ShortName) ?
                        $"-{argumentAttribute.ShortName}" : $"-{property.Name}";

                    var input = console.ReadValue($"Enter value for parameter {name}{settings.Separator}");

                    value = GetValue(property, argumentAttribute, new string[] { $"--{name}{settings.Separator}{input}" }) ??
                        throw new ArgumentNullException($"Value can't be null for property {property.Name}");
                }

                if (value != null)
                {
                    property.SetValue(command, value);
                }

            }
        }

        if (command is HelpCommand helpCommand)
        {
            var cmdName = GetCommandVerbName(verbs, helpCommand.Command);
            if (Commands.TryGetValue(cmdName, out var cmd) && cmd is not null)
                PrintCommand(cmd);
            else
                PrintAllCommands();
        }

        return command.Execute();

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
                .Select(param => dependencyResolver!.GetService(param.ParameterType))
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

    private object? GetValue(PropertyInfo property, OptionAttribute argumentAttribute, string[] args)
    {
        if (property is null) throw new ArgumentNullException(nameof(property));

        if (argumentAttribute is null) throw new ArgumentNullException(nameof(argumentAttribute));

        if (args is null) throw new ArgumentNullException(nameof(args));

        foreach (var arg in args)
        {
            var argProp = arg.Split(settings.Separator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (argProp.Length != 2)
            {
                throw new ArgumentOutOfRangeException(nameof(args));
            }

            var propName = argProp[0].Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            if (settings.NameComparer.Compare(argumentAttribute.LongName, propName) == 0 ||
                settings.NameComparer.Compare(argumentAttribute.ShortName, propName) == 0 ||
                settings.NameComparer.Compare(property.Name, propName) == 0)
            {
                if (property.PropertyType.IsPrimitive())
                {
                    return TypeConverter.ConvertString(argProp[1], property.PropertyType, settings.ParsingCulture);
                }

                if (property.PropertyType.IsEnum &&
                    Enum.TryParse(property.PropertyType, argProp[1], settings.NameComparer == StringComparer.OrdinalIgnoreCase, out var val))
                {
                    return val;
                }

                if (property.PropertyType == typeof(Guid) && Guid.TryParse(argProp[1], out var value))
                {
                    return value;
                }

            }
        }

        return null;


    }

    private string ReadCommandNameInteractive()
    {
        PrintAllCommands();

        return console.ReadValue("Command: ");
    }

    private void PrintAllCommands()
    {
        AssemblyHelper.PrintInfo(console);
        console.WriteLine("");
        console.WriteMessage("Commands: \n");
        foreach (var command in Commands)
        {
            PrintCommand(command.Value);
            console.WriteLine("");
        }
            
    }

    private void PrintCommand(Type commandType)
    {
        Verb verb = Verb.SelectFromType(commandType).Item1;
        var alias = verb.Aliases?.Any() == true ? $"({string.Join(",", verb.Aliases)})" : "";
        console.WriteMessage($"  {verb.Name} {alias}    {verb.HelpText} \n");
        console.WriteLine("");
        PrintParameters(commandType);
    }
    private void PrintParameters(Type commandType)
    {
        var properties = commandType.GetRuntimeProperties();
        if (properties.Any())
            console.WriteLine("  Options:");

        foreach (var property in properties)
        {
            var argumentAttribute = property.GetCustomAttribute<OptionAttribute>();
            if (argumentAttribute != null)
            {
                if (!string.IsNullOrWhiteSpace(argumentAttribute.LongName) &&
                    !string.IsNullOrWhiteSpace(argumentAttribute.ShortName))
                {
                    console.WriteMessage($"   --{argumentAttribute.LongName}{settings.Separator}<{property.PropertyType}> | -{argumentAttribute.ShortName}{settings.Separator}<{property.PropertyType}>    {argumentAttribute.HelpText} \n");
                }
                else if (!string.IsNullOrWhiteSpace(argumentAttribute.LongName))
                {
                    console.WriteMessage($"   --{argumentAttribute.LongName}{settings.Separator}<{property.PropertyType}>    {argumentAttribute.HelpText} \n");

                }
                else if (!string.IsNullOrWhiteSpace(argumentAttribute.ShortName))
                {
                    console.WriteMessage($"   -{argumentAttribute.ShortName}{settings.Separator}<{property.PropertyType}>    {argumentAttribute.HelpText} \n");

                }
                else
                {
                    console.WriteMessage($"   {property.Name}{settings.Separator}<{property.PropertyType}>    {argumentAttribute.HelpText} \n");
                }
            }

        }
    }


    private void AddDefaultCommands()
    {
        RegisterCommand(typeof(HelpCommand));
        RegisterCommand(typeof(VersionCommand));
    }

    private IConsole GetConsoleHost()
    {
        return settings.InteractiveMode ?
            new PrintHost()
            : new PrintHost();
    }

    private string GetCommandVerbName(IEnumerable<Tuple<Verb, Type>> verbs, string commandName)
    {
        return verbs.FirstOrDefault(vt =>
            settings.NameComparer.Equals(vt.Item1.Name, commandName)
            || vt.Item1.Aliases.Any(alias => settings.NameComparer.Equals(alias, commandName)))?.Item1?.Name ?? string.Empty;
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

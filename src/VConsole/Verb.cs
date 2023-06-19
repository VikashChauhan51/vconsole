using System.Reflection;

namespace VConsole;

public sealed class Verb
{
    public Verb(string name, string helpText, string[] aliases)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        HelpText = helpText ?? throw new ArgumentNullException(nameof(helpText));
        Aliases = aliases ?? new string[0];
    }

    public string Name { get; private set; }

    public string HelpText { get; private set; }

    public string[] Aliases { get; private set; }

    public static Verb FromAttribute(VerbAttribute attribute)
    {
        return new Verb(
            attribute.Name,
            attribute.HelpText,
            attribute.Aliases
            );
    }

    public static IEnumerable<Tuple<Verb, Type>> SelectFromTypes(IEnumerable<Type> types)
    {
        return from type in types
               let attrs = type.GetTypeInfo().GetCustomAttributes(typeof(VerbAttribute), true).ToArray()
               where attrs.Length == 1
               select Tuple.Create(
                   FromAttribute((VerbAttribute)attrs.Single()),
                   type);
    }
}

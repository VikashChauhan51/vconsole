namespace VConsole;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
public sealed class VerbAttribute : Attribute
{
    private string? helpText;
    private Type? resourceType;

    public VerbAttribute(string name, string[]? aliases = null, string? helpText = null, Type? resourceType = null)
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException(nameof(name));
        HelpText = helpText;
        ResourceType = resourceType;
        Aliases = aliases ?? new string[0];
    }
    public string Name { get; private set; }
    public string? HelpText
    {
        get { return helpText; }
        init
        {
            helpText = value;
        }
    }

    public Type? ResourceType
    {
        get { return resourceType; }
        init
        {
            resourceType = value;
        }
    }
    public string[] Aliases { get; private set; }
}

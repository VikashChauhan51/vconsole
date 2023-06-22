using System.Globalization;
using System.Reflection;

namespace VConsole;

public abstract class BaseAttribute : Attribute
{
    private object? @default;
    private Type? resourceType;
    private string? helpText;
    public bool Required
    {
        get;
        set;
    }
    public object? Default
    {
        get { return @default; }
        init
        {
            @default = value;
        }
    }

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
}

using System.Globalization;
using System.Reflection;
using VConsole.Core;

namespace VConsole;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class OptionAttribute : BaseAttribute
{
    private readonly string longName;
    private readonly string shortName;

    private OptionAttribute(string shortName, string longName) : base()
    {

        this.shortName = shortName ?? throw new ArgumentNullException(nameof(shortName));
        this.longName = longName ?? throw new ArgumentNullException(nameof(shortName));
    }

    public OptionAttribute(string longName)
        : this(string.Empty, longName)
    {
    }

    public OptionAttribute(char shortName, string longName)
        : this(shortName.ToOneCharString(), longName)
    {
    }

    public OptionAttribute(char shortName)
        : this(shortName.ToOneCharString(), string.Empty)
    {
    }


    public string LongName
    {
        get { return longName; }
    }

    public string ShortName
    {
        get { return shortName; }
    }
}

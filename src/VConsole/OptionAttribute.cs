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

    public override object? GetValue(PropertyInfo property, char separator, string[] args, StringComparer NameComparer, CultureInfo conversionCulture)
    {
        Validate(property, args, NameComparer, conversionCulture);

        foreach (var arg in args)
        {
            var argProp = arg.Split(separator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (argProp.Length != 2)
            {
                throw new ArgumentOutOfRangeException(nameof(args));
            }

            var propName = argProp[0].Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            if (NameComparer.Compare(this.longName, propName) == 0 || NameComparer.Compare(this.shortName, propName) == 0)
            {
                if (property.PropertyType.IsPrimitive())
                {
                    return TypeConverter.ConvertString(argProp[1], property.PropertyType, conversionCulture);
                }

                if (property.PropertyType.IsCustomStruct())
                {
                    return Enum.Parse(property.PropertyType, argProp[1], NameComparer == StringComparer.OrdinalIgnoreCase);
                }

            }
        }

        return null;
    }

    private void Validate(PropertyInfo property, string[] args, StringComparer NameComparer, CultureInfo conversionCulture)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        if (NameComparer is null)
        {
            throw new ArgumentNullException(nameof(NameComparer));
        }

        if (conversionCulture is null)
        {
            throw new ArgumentNullException(nameof(conversionCulture));
        }

        if (args is null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        if (string.IsNullOrWhiteSpace(this.longName) &&
            string.IsNullOrWhiteSpace(this.shortName))
        {
            throw new ArgumentException($"Please provide either [LongName] or [ShortName] of option parameter.");
        }
    }
}

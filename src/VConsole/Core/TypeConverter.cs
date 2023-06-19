using System.Globalization;
using ComponentModel = System.ComponentModel;

namespace VConsole.Core;

public static class TypeConverter
{

    public static object ConvertString(string value, Type type, CultureInfo conversionCulture)
    {
        try
        {
            return Convert.ChangeType(value, type, conversionCulture);
        }
        catch (InvalidCastException)
        {
            return ComponentModel.TypeDescriptor.GetConverter(type).ConvertFrom(null, conversionCulture, value);
        }
    }
}

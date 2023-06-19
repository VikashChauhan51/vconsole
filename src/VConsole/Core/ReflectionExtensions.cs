using System.Reflection;

namespace VConsole.Core;

internal static class ReflectionExtensions
{
    public static object CreateEmptyArray(this Type type)
    {
        return Array.CreateInstance(type, 0);
    }

    public static object? GetDefaultValue(this Type? type)
    {
        return type?.IsValueType == true ? Activator.CreateInstance(type) : null;
    }

    private static IEnumerable<Type> FlattenHierarchy(this Type? type)
    {
        if (type == null)
        {
            yield break;
        }
        yield return type;
        foreach (var @interface in type.SafeGetInterfaces())
        {
            yield return @interface;
        }
        foreach (var @interface in FlattenHierarchy(type?.GetTypeInfo().BaseType))
        {
            yield return @interface;
        }
    }

    private static IEnumerable<Type> SafeGetInterfaces(this Type? type)
    {
        return type == null ? Enumerable.Empty<Type>() : type.GetTypeInfo().GetInterfaces();
    }

    public static bool IsMutable(this Type? type)
    {
        if (type == null)
            return false;

        if (type == typeof(object))
            return true;

        var inheritedTypes = type.GetTypeInfo().FlattenHierarchy().Select(i => i.GetTypeInfo());

        foreach (var inheritedType in inheritedTypes)
        {
            if (
                inheritedType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(p => p.CanWrite) ||
                inheritedType.GetTypeInfo().GetFields(BindingFlags.Public | BindingFlags.Instance).Any()
                )
            {
                return true;
            }
        }

        return false;
    }

    public static object? CreateDefaultForImmutable(this Type? type)
    {
        if (type?.GetTypeInfo().IsGenericType == true && type?.GetTypeInfo().GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return type.GetTypeInfo().GetGenericArguments()[0].CreateEmptyArray();
        }
        return type?.GetDefaultValue();
    }

    public static object? StaticMethod(this Type? type, string name, params object[] args)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
            null,
            null,
            args);
    }

    public static object? StaticProperty(this Type? type, string name)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static,
            null,
            null,
            new object[] { });
    }

    public static object? InstanceProperty(this Type? type, string name, object target)
    {
        return type?.GetTypeInfo().InvokeMember(
            name,
            BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance,
            null,
            target,
            new object[] { });
    }

    public static bool IsPrimitive(this Type? type)
    {
        return
               (type?.GetTypeInfo().IsValueType == true && type != typeof(Guid))
            || type?.GetTypeInfo().IsPrimitive == true
            || new[] {
                     typeof(string)
                    ,typeof(decimal)
                    ,typeof(DateTime)
                    ,typeof(DateTimeOffset)
                    ,typeof(TimeSpan)
                    ,typeof(DateOnly)
                    ,typeof(TimeOnly)
               }.Contains(type)
            || Convert.GetTypeCode(type) != TypeCode.Object;
    }

    public static bool IsCustomStruct(this Type? type)
    {
        var isStruct = type?.GetTypeInfo().IsValueType == true &&
            !type?.GetTypeInfo().IsPrimitive == true &&
            !type?.GetTypeInfo().IsEnum == true && type != typeof(Guid);
        if (!isStruct) return false;
        var ctor = type?.GetTypeInfo().GetConstructor(new[] { typeof(string) });
        return ctor != null;
    }

}

namespace VConsole.Core;

internal static class EnumerableExtensions
{
    public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        var index = -1;
        foreach (var item in source)
        {
            index++;
            if (predicate(item))
            {
                break;
            }
        }
        return index;
    }

    public static object ToUntypedArray(this IEnumerable<object> value, Type type)
    {
        var array = Array.CreateInstance(type, value.Count());
        value.ToArray().CopyTo(array, 0);
        return array;
    }

    public static bool Empty<TSource>(this IEnumerable<TSource> source)
    {
        return !source.Any();
    }


    public static IEnumerable<T[]> Group<T>(this IEnumerable<T> source, int groupSize)
    {
        if (groupSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(groupSize));
        }

        T[] group = new T[groupSize];
        int groupIndex = 0;

        foreach (var item in source)
        {
            group[groupIndex++] = item;

            if (groupIndex == groupSize)
            {
                yield return group;

                group = new T[groupSize];
                groupIndex = 0;
            }
        }
    }
}

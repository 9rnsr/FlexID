namespace System
{
    public static class Extensions
    {
        public static string[] Split(this string value, StringSplitOptions options)
        {
            return value.Split(Array.Empty<char>(), options);
        }

        public static string[] Split(this string value, int count, StringSplitOptions options)
        {
            return value.Split(Array.Empty<char>(), count, options);
        }
    }
}
namespace System.Collections.Generic
{
    public static class Extensions
    {
        public static int IndexOf<T>(this IEnumerable<T> sources, Func<T, bool> predicate)
        {
            int index = 0;
            foreach (var value in sources)
            {
                if (predicate(value))
                    return index;
                index++;
            }
            return -1;
        }

        public static int IndexOf<T>(this IReadOnlyList<T> list, T element)
        {
            if (list is null)
                throw new ArgumentNullException(nameof(list));

            var comparer = EqualityComparer<T>.Default;
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                if (comparer.Equals(list[i], element))
                    return i;
            }
            return -1;
        }
    }
}

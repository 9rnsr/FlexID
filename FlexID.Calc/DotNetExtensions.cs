namespace System.Collections.Generic
{
    internal static class Extensions
    {
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

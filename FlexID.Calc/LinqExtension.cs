using System.Collections.Generic;

namespace System.Linq
{
    internal static class LinqExtension
    {
        private static (T1, T2) ZipSelector2<T1, T2>(T1 v1, T2 v2) => (v1, v2);

        public static IEnumerable<(T1, T2)> Zip<T1, T2>(this IEnumerable<T1> source1, IEnumerable<T2> source2)
        {
            if (source1 is null)
                throw new ArgumentNullException(nameof(source1));
            if (source2 is null)
                throw new ArgumentNullException(nameof(source2));

            return source1.Zip(source2, ZipSelector2);
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T element)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            yield return element;

            foreach (var item in source)
                yield return item;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T element)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            foreach (var item in source)
                yield return item;

            yield return element;
        }
    }
}

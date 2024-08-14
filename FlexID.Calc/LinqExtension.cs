using System.Collections.Generic;

namespace System.Linq
{
    internal static class LinqExtension
    {
        private static (T1, T2) ZipSelector2<T1, T2>(T1 v1, T2 v2) => (v1, v2);

        public static IEnumerable<(T1, T2)> Zip<T1, T2>(this IEnumerable<T1> source1, IEnumerable<T2> source2)
        {
            return source1.Zip(source2, ZipSelector2);
        }
    }
}

using System.Collections.Generic;

namespace NCoreUtils
{
    public static class AsyncLinqExtensions
    {
        public static async IAsyncEnumerable<T> Concat<T>(this IAsyncEnumerable<T> a, IAsyncEnumerable<T> b)
        {
            await foreach (var item in a)
            {
                yield return item;
            }
            await foreach (var item in b)
            {
                yield return item;
            }
        }
    }
}
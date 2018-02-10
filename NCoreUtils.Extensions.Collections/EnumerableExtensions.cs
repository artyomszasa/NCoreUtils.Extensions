using System;
using System.Collections.Generic;
using System.Linq;

namespace NCoreUtils
{
    /// <summary>
    /// Defines enumerable extension methods.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Maps collection to array. This method does the same as sequentially invoking <c>Select</c> and
        /// <c>ToArray</c> but creates array of the exact size if size of the input callection can be determined.
        /// </summary>
        /// <param name="source">Source collection.</param>
        /// <param name="selector">Selector function.</param>
        /// <returns>Created array.</returns>
        public static TResult[] MapToArray<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case TSource[] array:
                    return array.Map(selector);
                case ICollection<TSource> collection:
                    var result = new TResult[collection.Count];
                    var i = 0;
                    foreach (var item in collection)
                    {
                        result[i] = selector(item);
                        ++i;
                    }
                    return result;
                default:
                    return source.Select(selector).ToArray();
            }
        }
    }
}
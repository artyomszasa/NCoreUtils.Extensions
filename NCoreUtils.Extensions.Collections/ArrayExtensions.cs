using System;

namespace NCoreUtils
{
    /// <summary>
    /// Defines array extension methods.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Initializes new array of the specified size populating values using specified value factory function.
        /// </summary>
        /// <param name="count">Array size.</param>
        /// <param name="valueFactory">Value factory function.</param>
        /// <returns>Initialized array.</returns>
        public static T[] Initialize<T>(int count, Func<int, T> valueFactory)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }
            var result = new T[count];
            for (var i = 0; i < count; ++i)
            {
                result[i] = valueFactory(i);
            }
            return result;
        }

        /// <summary>
        /// Initializes new array of the same size as the input array populating resulting array values by applying
        /// mapper function to the values of the input array.
        /// </summary>
        /// <param name="source">Input array.</param>
        /// <param name="mapper">Selector function.</param>
        /// <returns>Initialized array.</returns>
        public static TResult[] Map<TSource, TResult>(this TSource[] source, Func<TSource, TResult> mapper)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            var result = new TResult[source.Length];
            for (var i = 0; i < source.Length; ++i)
            {
                result[i] = mapper(source[i]);
            }
            return result;
        }
    }
}
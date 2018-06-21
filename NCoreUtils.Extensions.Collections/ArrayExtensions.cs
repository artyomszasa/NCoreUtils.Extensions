using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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

        /// <summary>
        /// Sets <paramref name="array" /> as the value for all the elements in the array object.
        /// </summary>
        /// <param name="array">Array to fill.</param>
        /// <param name="value">Value to fill array with.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void Fill<T>(this T[] array, T value = default(T))
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            for (int i = 0; i < array.Length; ++i)
            {
                array[i] = value;
            }
        }

        /// <summary>
        /// Creates new array and copies specified slice of the array into the newly created array.
        /// </summary>
        /// <param name="array">Source array.</param>
        /// <param name="offset">Index of the first copied item.</param>
        /// <param name="count">Copied items count.</param>
        /// <returns>Newly created array.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static T[] Slice<T>(this T[] array, int offset, int count = -1)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (-1 == count)
            {
                count = array.Length - offset;
            }
            var result = new T[count];
            Array.Copy(array, offset, result, 0, count);
            return result;
        }

        /// <summary>
        /// Returns the value associated with the specified key, or a default value if the dictionary has no value
        /// assiciated to the key.
        /// </summary>
        /// <param name="source">Source dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="valueFactory">Value factory invoked to provide default value.</param>
        /// <returns>
        /// Either the value associated with the specified key, or a default value if the dictionary has no value
        /// assiciated to the key.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static TValue GetOrSupply<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source, TKey key, Func<TValue> valueFactory)
        {
            TValue value;
            if (!source.TryGetValue(key, out value))
            {
                return valueFactory();
            }
            return value;
        }

        /// <summary>
        /// Removes and returns the last item of the specified list.
        /// </summary>
        /// <param name="list">Source list.</param>
        /// <returns>Former last item of the list.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown if list is empty.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static T Pop<T>(this List<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (0 == list.Count)
            {
                throw new InvalidOperationException("List is empty.");
            }
            var index = list.Count - 1;
            var result = list[index];
            list.RemoveAt(index);
            return result;
        }

        /// <summary>
        /// Finds minimum value using specified comparer or default comparer if not specified.
        /// </summary>
        /// <param name="source">Source enumerable.</param>
        /// <param name="comparer">Comparer.</param>
        /// <returns>Minimum value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T MinBy<T>(this IEnumerable<T> source, IComparer<T> comparer = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var cmp = comparer ?? Comparer<T>.Default;
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence is empty.");
                }
                var min = enumerator.Current;
                while (enumerator.MoveNext())
                {
                    var curr = enumerator.Current;
                    if (0 > cmp.Compare(curr, min))
                    {
                        min = curr;
                    }
                }
                return min;
            }
        }

        /// <summary>
        /// Finds minimum value using specified key selector and key comparer or default comparer if not specified.
        /// </summary>
        /// <param name="source">Source enumerable.</param>
        /// <param name="selector">Key selector.</param>
        /// <param name="comparer">Key comparer.</param>
        /// <returns>Minimum value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue MinBy<TValue, TKey>(this IEnumerable<TValue> source, Func<TValue, TKey> selector, IComparer<TKey> comparer = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var cmp = comparer ?? Comparer<TKey>.Default;
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence is empty.");
                }
                var min = enumerator.Current;
                var minKey = selector(min);
                while (enumerator.MoveNext())
                {
                    var curr = enumerator.Current;
                    var currKey = selector(curr);
                    if (0 > cmp.Compare(currKey, minKey))
                    {
                        min = curr;
                        minKey = currKey;
                    }
                }
                return min;
            }
        }
    }
}
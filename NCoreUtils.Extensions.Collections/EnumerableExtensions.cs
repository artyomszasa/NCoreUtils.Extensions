using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NCoreUtils
{
    /// <summary>
    /// Defines enumerable extension methods.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Maps collection to array. This method does the same as sequentially invoking <c>Select</c> and
        /// <c>ToArray</c> but creates array of the exact size if size of the input sequence can be determined.
        /// </summary>
        /// <param name="source">Source collection.</param>
        /// <param name="selector">Selector function.</param>
        /// <returns>Created array.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                case IList<TSource> list:
                    {
                        var result = new TResult[list.Count];
                        for (var i = 0; i < list.Count; ++i)
                        {
                            result[i] = selector(list[i]);
                        }
                        return result;
                    }
                case IReadOnlyList<TSource> list:
                    {
                        var result = new TResult[list.Count];
                        for (var i = 0; i < list.Count; ++i)
                        {
                            result[i] = selector(list[i]);
                        }
                        return result;
                    }
                case ICollection<TSource> collection:
                    {
                        var result = new TResult[collection.Count];
                        var i = 0;
                        foreach (var item in collection)
                        {
                            result[i++] = selector(item);
                        }
                        return result;
                    }
                default:
                    return source.Select(selector).ToArray();
            }
        }

        /// <summary>
        /// Gets the first element of a sequence.
        /// </summary>
        /// <param name="source">A sequence to return first element from.</param>
        /// <param name="item">
        /// When method returns, the first element of sequence, if any.
        /// </param>
        /// <typeparam name="T">
        /// The type of the elements of <paramref name="source" />.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if sequence was not empty and its first element has
        /// been stored in <paramref name="item" />, <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetFirst<T>(this IEnumerable<T> source, out T item)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case IList<T> list:
                    switch (list.Count)
                    {
                        case 0:
                            item = default!;
                            return false;
                        default:
                            item = list[0];
                            return true;
                    }
                case IReadOnlyList<T> list:
                    switch (list.Count)
                    {
                        case 0:
                            item = default!;
                            return false;
                        default:
                            item = list[0];
                            return true;
                    }
                default:
                using (var enumerator = source.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        item = enumerator.Current;
                        return true;
                    }
                    item = default!;
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the first element of a sequence that satisfies a condition.
        /// </summary>
        /// <param name="source">A sequence to return first element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="item">
        /// When method returns, the first element of sequence that satisfies a condition, if any.
        /// </param>
        /// <typeparam name="T">
        /// The type of the elements of <paramref name="source" />.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if element was found and has been stored in <paramref name="item" />,
        /// <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetFirst<T>(this IEnumerable<T> source, Func<T, bool> predicate, out T item)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(predicate));
                case IList<T> list:
                    {
                        if (0 == list.Count)
                        {
                            item = default!;
                            return false;
                        }
                        var found = false;
                        item = default!;
                        for (var i = 0; !found && i < list.Count; ++i)
                        {
                            var current = list[i];
                            if (predicate(current))
                            {
                                found = true;
                                item = current;
                            }
                        }
                        return found;
                    }
                case IReadOnlyList<T> list:
                    {
                        if (0 == list.Count)
                        {
                            item = default!;
                            return false;
                        }
                        var found = false;
                        item = default!;
                        for (var i = 0; !found && i < list.Count; ++i)
                        {
                            var current = list[i];
                            if (predicate(current))
                            {
                                found = true;
                                item = current;
                            }
                        }
                        return found;
                    }
                default:
                    using (var enumerator = source.GetEnumerator())
                    {
                        var found = false;
                        item = default!;
                        while (!found && enumerator.MoveNext())
                        {
                            var current = enumerator.Current;
                            if (predicate(current))
                            {
                                found = true;
                                item = current;
                            }
                        }
                        return found;
                    }
            }
        }

        /// <summary>
        /// Gets the only element of a sequence, and throws an exception if there is not exactly one element in the
        /// sequence.
        /// </summary>
        /// <param name="source">A sequence to return element from.</param>
        /// <param name="item">
        /// When method returns, the element of sequence, if any.
        /// </param>
        /// <typeparam name="T">
        /// The type of the elements of <paramref name="source" />.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if sequence contained a single element and the element has
        /// been stored in <paramref name="item" />, <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if sequence contains more that one element.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetSingle<T>(this IEnumerable<T> source, out T item)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case IList<T> list:
                    switch (list.Count)
                    {
                        case 0:
                            item = default!;
                            return false;
                        case 1:
                            item = list[0];
                            return true;
                        default:
                            throw new InvalidOperationException("Sequence contains more than one element.");
                    }
                case IReadOnlyList<T> list:
                    switch (list.Count)
                    {
                        case 0:
                            item = default!;
                            return false;
                        case 1:
                            item = list[0];
                            return true;
                        default:
                            throw new InvalidOperationException("Sequence contains more than one element.");
                    }
                default:
                    using (var enumerator = source.GetEnumerator())
                    {
                        if (!enumerator.MoveNext())
                        {
                            item = default!;
                            return false;
                        }
                        var result = enumerator.Current;
                        if (enumerator.MoveNext())
                        {
                            throw new InvalidOperationException("Sequence contains more than one element.");
                        }
                        item = result;
                        return true;
                    }
            }
        }

        /// <summary>
        /// Gets the single element of a sequence that satisfies a condition.
        /// </summary>
        /// <param name="source">A sequence to return element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="item">
        /// When method returns, the only element of sequence that satisfies a condition, if any.
        /// </param>
        /// <typeparam name="T">
        /// The type of the elements of <paramref name="source" />.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if the only element that satisfies a condition was found and has been stored in
        /// <paramref name="item" />, <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if sequence contains more that one element that satisfies a condition.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetSingle<T>(this IEnumerable<T> source, Func<T, bool> predicate, out T item)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(predicate));
                case IList<T> list:
                    {
                        if (0 == list.Count)
                        {
                            item = default!;
                            return false;
                        }
                        var found = false;
                        item = default!;
                        for (var i = 0; i < list.Count; ++i)
                        {
                            var current = list[i];
                            if (predicate(current))
                            {
                                if (found)
                                {
                                    throw new InvalidOperationException("Sequence contains more than one element that satisfies the specified condition.");
                                }
                                found = true;
                                item = current;
                            }
                        }
                        return found;
                    }
                case IReadOnlyList<T> list:
                    {
                        if (0 == list.Count)
                        {
                            item = default!;
                            return false;
                        }
                        var found = false;
                        item = default!;
                        for (var i = 0; i < list.Count; ++i)
                        {
                            var current = list[i];
                            if (predicate(current))
                            {
                                if (found)
                                {
                                    throw new InvalidOperationException("Sequence contains more than one element that satisfies the specified condition.");
                                }
                                found = true;
                                item = current;
                            }
                        }
                        return found;
                    }
                default:
                    using (var enumerator = source.GetEnumerator())
                    {
                        var found = false;
                        item = default!;
                        while (enumerator.MoveNext())
                        {
                            var current = enumerator.Current;
                            if (predicate(current))
                            {
                                if (found)
                                {
                                    throw new InvalidOperationException("Sequence contains more than one element that satisfies the specified condition.");
                                }
                                found = true;
                                item = current;
                            }
                        }
                        return found;
                    }
            }
        }


        /// <summary>
        /// Gets the last element of a sequence.
        /// </summary>
        /// <param name="source">A sequence to return last element from.</param>
        /// <param name="item">
        /// When method returns, the last element of sequence, if any.
        /// </param>
        /// <typeparam name="T">
        /// The type of the elements of <paramref name="source" />.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if sequence was not empty and its last element has
        /// been stored in <paramref name="item" />, <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetLast<T>(this IEnumerable<T> source, out T item)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case IList<T> list:
                    switch (list.Count)
                    {
                        case 0:
                            item = default!;
                            return false;
                        default:
                            item = list[list.Count - 1];
                            return true;
                    }
                case IReadOnlyList<T> list:
                    switch (list.Count)
                    {
                        case 0:
                            item = default!;
                            return false;
                        default:
                            item = list[list.Count - 1];
                            return true;
                    }
                default:
                using (var enumerator = source.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        var result = enumerator.Current;
                        while (enumerator.MoveNext())
                        {
                            result = enumerator.Current;
                        }
                        item = result;
                        return true;
                    }
                    item = default!;
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the last element of a sequence that satisfies a condition.
        /// </summary>
        /// <param name="source">A sequence to return last element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="item">
        /// When method returns, the last element of sequence that satisfies a condition, if any.
        /// </param>
        /// <typeparam name="T">
        /// The type of the elements of <paramref name="source" />.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if element was found and has been stored in <paramref name="item" />,
        /// <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetLast<T>(this IEnumerable<T> source, Func<T, bool> predicate, out T item)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(predicate));
                case IList<T> list:
                    {
                        if (0 == list.Count)
                        {
                            item = default!;
                            return false;
                        }
                        var found = false;
                        item = default!;
                        for (var i = list.Count - 1; !found && i >= 0; --i)
                        {
                            var current = list[i];
                            if (predicate(current))
                            {
                                found = true;
                                item = current;
                            }
                        }
                        return found;
                    }
                case IReadOnlyList<T> list:
                    {
                        if (0 == list.Count)
                        {
                            item = default!;
                            return false;
                        }
                        var found = false;
                        item = default!;
                        for (var i = list.Count - 1; !found && i >= 0; --i)
                        {
                            var current = list[i];
                            if (predicate(current))
                            {
                                found = true;
                                item = current;
                            }
                        }
                        return found;
                    }
                default:
                    return source.Where(predicate).TryGetLast(out item);
            }
        }

        /// <summary>
        /// Finds minimum value using specified comparer or default comparer if not specified.
        /// </summary>
        /// <param name="source">Source enumerable.</param>
        /// <param name="comparer">Comparer.</param>
        /// <returns>Minimum value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T MinBy<T>(this IEnumerable<T> source, IComparer<T>? comparer = default)
        {
            var cmp = comparer ?? Comparer<T>.Default;
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case T[] array:
                    {
                        if (0 == array.Length)
                        {
                            throw new InvalidOperationException("Sequence is empty.");
                        }
                        var min = array[0];
                        for (var i = 1; i < array.Length; ++i)
                        {
                            var curr = array[i];
                            if (0 > cmp.Compare(curr, min))
                            {
                                min = curr;
                            }
                        }
                        return min;
                    }
                case IList<T> list:
                    {
                        if (0 == list.Count)
                        {
                            throw new InvalidOperationException("Sequence is empty.");
                        }
                        var min = list[0];
                        for (var i = 1; i < list.Count; ++i)
                        {
                            var curr = list[i];
                            if (0 > cmp.Compare(curr, min))
                            {
                                min = curr;
                            }
                        }
                        return min;
                    }
                case IReadOnlyList<T> list:
                    {
                        if (0 == list.Count)
                        {
                            throw new InvalidOperationException("Sequence is empty.");
                        }
                        var min = list[0];
                        for (var i = 1; i < list.Count; ++i)
                        {
                            var curr = list[i];
                            if (0 > cmp.Compare(curr, min))
                            {
                                min = curr;
                            }
                        }
                        return min;
                    }
                default:
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
        }

        /// <summary>
        /// Finds minimum value using specified key selector and key comparer or default comparer if not specified.
        /// </summary>
        /// <param name="source">Source enumerable.</param>
        /// <param name="selector">Key selector.</param>
        /// <param name="comparer">Key comparer.</param>
        /// <returns>Minimum value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue MinBy<TValue, TKey>(this IEnumerable<TValue> source, Func<TValue, TKey> selector, IComparer<TKey>? comparer = default)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var cmp = comparer ?? Comparer<TKey>.Default;
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case TValue[] array:
                    {
                        if (0 == array.Length)
                        {
                            throw new InvalidOperationException("Sequence is empty.");
                        }
                        var min = array[0];
                        var minKey = selector(array[0]);
                        for (var i = 1; i < array.Length; ++i)
                        {
                            var curr = array[i];
                            var currKey = selector(curr);
                            if (0 > cmp.Compare(currKey, minKey))
                            {
                                min = curr;
                                minKey = currKey;
                            }
                        }
                        return min;
                    }
                case IReadOnlyList<TValue> list:
                    {
                        if (0 == list.Count)
                        {
                            throw new InvalidOperationException("Sequence is empty.");
                        }
                        var min = list[0];
                        var minKey = selector(list[0]);
                        for (var i = 1; i < list.Count; ++i)
                        {
                            var curr = list[i];
                            var currKey = selector(curr);
                            if (0 > cmp.Compare(currKey, minKey))
                            {
                                min = curr;
                                minKey = currKey;
                            }
                        }
                        return min;
                    }
                case IList<TValue> list:
                    {
                        if (0 == list.Count)
                        {
                            throw new InvalidOperationException("Sequence is empty.");
                        }
                        var min = list[0];
                        var minKey = selector(list[0]);
                        for (var i = 1; i < list.Count; ++i)
                        {
                            var curr = list[i];
                            var currKey = selector(curr);
                            if (0 > cmp.Compare(currKey, minKey))
                            {
                                min = curr;
                                minKey = currKey;
                            }
                        }
                        return min;
                    }
                default:
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

        /// <summary>
        /// Finds maximum value using specified comparer or default comparer if not specified.
        /// </summary>
        /// <param name="source">Source enumerable.</param>
        /// <param name="comparer">Comparer.</param>
        /// <returns>Maximum value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T MaxBy<T>(this IEnumerable<T> source, IComparer<T>? comparer = default)
        {
            var cmp = comparer ?? Comparer<T>.Default;
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case T[] array:
                    {
                        if (0 == array.Length)
                        {
                            throw new InvalidOperationException("Sequence is empty.");
                        }
                        var min = array[0];
                        for (var i = 1; i < array.Length; ++i)
                        {
                            var curr = array[i];
                            if (0 < cmp.Compare(curr, min))
                            {
                                min = curr;
                            }
                        }
                        return min;
                    }
                case IList<T> list:
                    {
                        if (0 == list.Count)
                        {
                            throw new InvalidOperationException("Sequence is empty.");
                        }
                        var min = list[0];
                        for (var i = 1; i < list.Count; ++i)
                        {
                            var curr = list[i];
                            if (0 < cmp.Compare(curr, min))
                            {
                                min = curr;
                            }
                        }
                        return min;
                    }
                case IReadOnlyList<T> list:
                    {
                        if (0 == list.Count)
                        {
                            throw new InvalidOperationException("Sequence is empty.");
                        }
                        var min = list[0];
                        for (var i = 1; i < list.Count; ++i)
                        {
                            var curr = list[i];
                            if (0 < cmp.Compare(curr, min))
                            {
                                min = curr;
                            }
                        }
                        return min;
                    }
                default:
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
                            if (0 < cmp.Compare(curr, min))
                            {
                                min = curr;
                            }
                        }
                        return min;
                    }
            }
        }

        /// <summary>
        /// Finds maximum value using specified key selector and key comparer or default comparer if not specified.
        /// </summary>
        /// <param name="source">Source enumerable.</param>
        /// <param name="selector">Key selector.</param>
        /// <param name="comparer">Key comparer.</param>
        /// <returns>Maximum value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue MaxBy<TValue, TKey>(this IEnumerable<TValue> source, Func<TValue, TKey> selector, IComparer<TKey>? comparer = default)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var cmp = comparer ?? Comparer<TKey>.Default;
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case TValue[] array:
                    {
                        if (0 == array.Length)
                        {
                            throw new InvalidOperationException("Sequence is empty.");
                        }
                        var min = array[0];
                        var minKey = selector(array[0]);
                        for (var i = 1; i < array.Length; ++i)
                        {
                            var curr = array[i];
                            var currKey = selector(curr);
                            if (0 < cmp.Compare(currKey, minKey))
                            {
                                min = curr;
                                minKey = currKey;
                            }
                        }
                        return min;
                    }
                case IReadOnlyList<TValue> list:
                    {
                        if (0 == list.Count)
                        {
                            throw new InvalidOperationException("Sequence is empty.");
                        }
                        var min = list[0];
                        var minKey = selector(list[0]);
                        for (var i = 1; i < list.Count; ++i)
                        {
                            var curr = list[i];
                            var currKey = selector(curr);
                            if (0 < cmp.Compare(currKey, minKey))
                            {
                                min = curr;
                                minKey = currKey;
                            }
                        }
                        return min;
                    }
                case IList<TValue> list:
                    {
                        if (0 == list.Count)
                        {
                            throw new InvalidOperationException("Sequence is empty.");
                        }
                        var min = list[0];
                        var minKey = selector(list[0]);
                        for (var i = 1; i < list.Count; ++i)
                        {
                            var curr = list[i];
                            var currKey = selector(curr);
                            if (0 < cmp.Compare(currKey, minKey))
                            {
                                min = curr;
                                minKey = currKey;
                            }
                        }
                        return min;
                    }
                default:
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
                            if (0 < cmp.Compare(currKey, minKey))
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
}
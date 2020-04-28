using System;
using System.Collections.Generic;

namespace NCoreUtils
{
    /// <summary>
    /// Read-only list extensions.
    /// </summary>
    public static class ReadOnlyListExtensions
    {
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the
        /// zero-based index of the first occurrence within the entire list.
        /// </summary>
        /// <param name="source">List to search.</param>
        /// <param name="match">The predicate that defines the conditions of the element to search for.</param>
        /// <typeparam name="T">Type of list item.</typeparam>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by
        /// <paramref name="match" />, if found; otherwise, <c>-1</c>.
        /// </returns>
        public static int FindIndex<T>(this IReadOnlyList<T> source, Predicate<T> match)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case T[] array:
                    return Array.FindIndex(array, match);
                case List<T> list:
                    return list.FindIndex(match);
                default:
                    if (0 == source.Count)
                    {
                        return -1;
                    }
                    for (var i = 0; i < source.Count; ++i)
                    {
                        if (match(source[i]))
                        {
                            return i;
                        }
                    }
                    return -1;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NCoreUtils
{
    /// <summary>
    /// Defines list extension methods.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Removes and returns the last item of the specified list.
        /// </summary>
        /// <param name="list">Source list.</param>
        /// <returns>Former last item of the list.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown if list is empty.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    }
}
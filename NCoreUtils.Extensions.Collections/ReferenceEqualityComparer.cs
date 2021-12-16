using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NCoreUtils
{
    /// <summary>
    /// Implements equality comparison using object identity (reference).
    /// </summary>
    /// <typeparam name="T">Item type.</typeparam>
    public class ReferenceEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        /// <summary>
        /// Gets shared instance of the reference equality comparer.
        /// </summary>
        public static ReferenceEqualityComparer<T> Instance { get; } = new ReferenceEqualityComparer<T>();

        ReferenceEqualityComparer() { }

        /// <summary>
        /// Checks whether the spacified objects are the same using <c>ReferenceEquals</c>.
        /// </summary>
        /// <param name="x">First object.</param>
        /// <param name="y">Second object.</param>
        public bool Equals(T? x, T? y)
            => ReferenceEquals(x, y);

        /// <summary>
        /// Gets hash code of the object using <c>RuntimeHelpers.GetHashCode</c>.
        /// </summary>
        /// <param name="obj"></param>
        public int GetHashCode(T obj)
            => RuntimeHelpers.GetHashCode(obj);
    }
}
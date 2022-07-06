using System.Collections.Generic;

namespace NCoreUtils.Collections
{
    /// <summary>
    /// Represents a dictionary which may contain multiple values assigned to the same key.
    /// </summary>
    public interface IMultiValueDictionary<TKey, TValue>
        : IReadOnlyMultiValueDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        /// <summary>
        /// Assignes the specified value to the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        void Add(TKey key, TValue value);

        /// <summary>
        /// Assignes the specified values to the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="values">Values to assign.</param>
        void Add(TKey key, IEnumerable<TValue> values);

        /// <summary>
        /// Assignes the specified values to the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="values">Values to assign.</param>
        void Add(TKey key, params TValue[] values);

        /// <summary>
        /// Assignes the specified value to the specified key. Replaces any previous assignments.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Values to assign.</param>
        void Set(TKey key, TValue value);

        /// <summary>
        /// Assignes the specified values to the specified key. Replaces any previous assignments.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="values">Values to assign.</param>
        void Set(TKey key, IEnumerable<TValue> values);

        /// <summary>
        /// Assignes the specified values to the specified key. Replaces any previous assignments.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="values">Values to assign.</param>
        void Set(TKey key, params TValue[] values);

        /// <summary>
        /// Removes all values assigned to the specified key.
        /// </summary>
        /// <returns>Amount of values has been removed.</returns>
        /// <param name="key">Key.</param>
        int Remove(TKey key);
    }
}
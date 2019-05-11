using System.Collections;
using System.Collections.Generic;

namespace NCoreUtils.Collections
{
    /// <summary>
    /// Represents a read-only dictionary which may contain multiple values assigned to the same key.
    /// </summary>
    public interface IReadOnlyMultiValueDictionary<TKey, TValue>
        : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// Gets the sequence containing all keys which are assigned at least one value.
        /// </summary>
        /// <value>The keys.</value>
        IEnumerable<TKey> Keys { get; }

        /// <summary>
        /// Gets whether sepcified key has assigned value.
        /// </summary>
        /// <returns><c>true</c>, if key has value assigned, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        bool ContainsKey(TKey key);

        /// <summary>
        /// Gets all values assigned to the specified key.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if at least one value has been assigned the the specified key, <c>false</c> otherwise.
        /// </returns>
        /// <param name="key">Key.</param>
        /// <param name="values">
        /// Variable to store assigned values. After calling this function either references array containing the assigned
        /// values or null.
        /// </param>
        bool TryGetValues(TKey key, out TValue[] values);

        /// <summary>
        /// Adds all values assigned to the specified key to the specified collection.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if at least one value has been assigned the the specified key, <c>false</c> otherwise.
        /// </returns>
        /// <param name="key">Key.</param>
        /// <param name="values">Collection to add assigned values to.</param>
        bool TryGetValues(TKey key, ICollection<TValue> values);

        /// <summary>
        /// Tries the get value for key. If multiple values are present first found value is returned.
        /// </summary>
        /// <param name="key">Key to search values by.</param>
        /// <param name="value">Returned value if found.</param>
        bool TryGetValue(TKey key, out TValue value);
    }
}
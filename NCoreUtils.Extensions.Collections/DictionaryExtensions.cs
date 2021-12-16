using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NCoreUtils
{
    /// <summary>
    /// Defines dictionary extension methods.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Either returns value associated with the specified key or the default value.
        /// </summary>
        /// <param name="dictionary">Source dictionary.</param>
        /// <param name="key">Key to try.</param>
        /// <param name="defaultValue">Value returned if no value associated with the specified key.</param>
        /// <returns>Either value associated with the specified key or the default value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue GetOrDefault<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue = default)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }
            return defaultValue;
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
        public static TValue GetOrSupply<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source, TKey key, Func<TValue> valueFactory)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (valueFactory == null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }
            return source.TryGetValue(key, out var value) ? value : valueFactory();
        }
    }
}
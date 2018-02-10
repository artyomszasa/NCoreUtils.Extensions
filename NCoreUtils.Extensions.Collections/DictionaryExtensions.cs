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
            TValue defaultValue = default(TValue))
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
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NCoreUtils.Collections
{
    /// <summary>
    /// Implements read-only dictionary that maps the source dictionary elements on access using the specified mapping.
    /// Mapping is performed unconditionaly on every access, no caching applied.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TSource">Source element type.</typeparam>
    /// <typeparam name="TResult">Exposed element type.</typeparam>
    public class MappingReadOnlyDictionary<TKey, TSource, TResult> : IReadOnlyDictionary<TKey, TResult>
    {
        readonly IReadOnlyDictionary<TKey, TSource> _source;
        readonly Func<TSource, TResult> _mapping;

        /// <summary>
        /// Initializes new instance from the specified arguments.
        /// </summary>
        /// <param name="source">Source dictionary.</param>
        /// <param name="mapping">Mapping to apply.</param>
        public MappingReadOnlyDictionary(IReadOnlyDictionary<TKey, TSource> source, Func<TSource, TResult> mapping)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));
        }

        /// <summary>
        /// Gets the mapped element that has the specified key in the read-only dictionary.
        /// </summary>
        public TResult this[TKey key] => _mapping(_source[key]);

        /// <summary>
        /// Gets an enumerable collection that contains the keys in the read-only dictionary.
        /// </summary>
        public IEnumerable<TKey> Keys => _source.Keys;

        /// <summary>
        /// Gets an enumerable collection that contains the mapped values of the source read-only dictionary.
        /// </summary>
        public IEnumerable<TResult> Values => _source.Values.Select(_mapping);

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count => _source.Count;

        /// <summary>
        /// Determines whether the read-only dictionary contains an element that has the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>
        /// <c>true</c> if the read-only dictionary contains an element that has the specified key; otherwise,
        /// <c>false</c>.
        /// </returns>
        public bool ContainsKey(TKey key) => _source.ContainsKey(key);

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the mappeds collection.</returns>
        public IEnumerator<KeyValuePair<TKey, TResult>> GetEnumerator()
            => _source.GetEnumerator()
                .Select(kv => new KeyValuePair<TKey, TResult>(kv.Key, _mapping(kv.Value)));

        /// <summary>
        /// Gets the mapped value that is associated with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">
        /// When this method returns, the mapped value associated with the specified key, if the key is found;
        /// otherwise, the default value for the type of the <paramref name="value" /> parameter. This
        /// parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <c>true</c> if the read-onyl dictionary contains an element that has the specified key; otherwise,
        /// <c>false</c>.
        /// </returns>
        public bool TryGetValue(TKey key, out TResult value)
        {
            if (_source.TryGetValue(key, out var v))
            {
                value = _mapping(v);
                return true;
            }
            value = default(TResult);
            return false;
        }

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
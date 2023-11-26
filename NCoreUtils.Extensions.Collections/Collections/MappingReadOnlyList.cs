using System;
using System.Collections;
using System.Collections.Generic;

namespace NCoreUtils.Collections;

/// <summary>
/// Implements read-only list that maps the source list elements on access using the specified mapping. Mapping is
/// performed unconditionaly on every access, no caching applied.
/// </summary>
/// <typeparam name="TSource">Source element type.</typeparam>
/// <typeparam name="TResult">Target element type.</typeparam>
/// <remarks>
/// Initializes new instance from the specified arguments.
/// </remarks>
/// <param name="source">Source list.</param>
/// <param name="mapping">Mapping to apply.</param>
public class MappingReadOnlyList<TSource, TResult>(IReadOnlyList<TSource> source, Func<TSource, TResult> mapping) : IReadOnlyList<TResult>
{
    private readonly IReadOnlyList<TSource> _source = source ?? throw new ArgumentNullException(nameof(source));

    private readonly Func<TSource, TResult> _mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));

    /// <summary>
    /// Gets the mapped element at the specified index in the read-only list.
    /// </summary>
    public TResult this[int index] => _mapping(_source[index]);

    /// <summary>
    /// Gets the number of elements in the collection.
    /// </summary>
    public int Count => _source.Count;

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the mapped collection.</returns>
    public IEnumerator<TResult> GetEnumerator() => _source.GetEnumerator().Select(_mapping);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
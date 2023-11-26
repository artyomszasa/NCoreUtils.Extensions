using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NCoreUtils;

/// <summary>
/// Defines array extension methods.
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// Initializes new array of the specified size populating values using specified value factory function.
    /// </summary>
    /// <param name="count">Array size.</param>
    /// <param name="valueFactory">Value factory function.</param>
    /// <returns>Initialized array.</returns>
    public static T[] Initialize<T>(int count, Func<int, T> valueFactory)
    {
        if (valueFactory == null)
        {
            throw new ArgumentNullException(nameof(valueFactory));
        }
        var result = new T[count];
        for (var i = 0; i < count; ++i)
        {
            result[i] = valueFactory(i);
        }
        return result;
    }

    /// <summary>
    /// Initializes new array of the same size as the input array populating resulting array values by applying
    /// mapper function to the values of the input array.
    /// </summary>
    /// <param name="source">Input array.</param>
    /// <param name="mapper">Selector function.</param>
    /// <returns>Initialized array.</returns>
    public static TResult[] Map<TSource, TResult>(this TSource[] source, Func<TSource, TResult> mapper)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (mapper == null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }
        var result = new TResult[source.Length];
        for (var i = 0; i < source.Length; ++i)
        {
            result[i] = mapper(source[i]);
        }
        return result;
    }

    /// <summary>
    /// Sets <paramref name="array" /> as the value for all the elements in the array object.
    /// </summary>
    /// <param name="array">Array to fill.</param>
    /// <param name="value">Value to fill array with.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
#if NET6_0_OR_GREATER
    [Obsolete("Use Array.Fill instead")]
#endif
    public static void Fill<T>(this T[] array, T value = default!)
#if NET6_0_OR_GREATER
        => Array.Fill(array, value);
#else
    {

        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }
        for (int i = 0; i < array.Length; ++i)
        {
            array[i] = value;
        }
    }
#endif

    /// <summary>
    /// Creates new array and copies specified slice of the array into the newly created array.
    /// </summary>
    /// <param name="array">Source array.</param>
    /// <param name="offset">Index of the first copied item.</param>
    /// <param name="count">Copied items count.</param>
    /// <returns>Newly created array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    [Obsolete("Use range operator instead")]
    public static T[] Slice<T>(this T[] array, int offset, int count = -1)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }
        if (-1 == count)
        {
            count = array.Length - offset;
        }
        var result = new T[count];
        Array.Copy(array, offset, result, 0, count);
        return result;
    }
}
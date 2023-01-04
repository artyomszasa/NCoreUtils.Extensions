using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NCoreUtils
{
    /// <summary>
    /// Defines nullable extension methods.
    /// </summary>
    public static class NullableExtensions
    {
        /// <summary>
        /// Converts actually stored value if present creating either empty value of the target type or value
        /// containing result of the conversion.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <param name="selector">Conversion function.</param>
        /// <returns>
        /// Either empty value of the target type or value containing result of the conversion.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static TResult? Map<TSource, TResult>(this TSource? source, Func<TSource, TResult> selector)
            where TSource : struct
            where TResult : struct
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            return source is TSource value ? selector(value) : default(TResult?);
        }

        /// <summary>
        /// Returns result of invoking <paramref name="binder" /> on the actually stored value if value is present,
        /// empty value of the result type otherwise.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <param name="binder">Function to invoke if value is present.</param>
        /// <returns>
        /// Either result of invoking <paramref name="binder" /> on the actually stored value if value is present or
        /// empty value of the result type
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static TResult? Bind<TSource, TResult>(this TSource? source, Func<TSource, TResult?> binder)
            where TSource : struct
            where TResult : struct
        {
            if (binder == null)
            {
                throw new ArgumentNullException(nameof(binder));
            }
            return source is TSource value ? binder(value) : default;
        }

        /// <summary>
        /// Filters value so that the returned value is either empty or satisfies the specified predicate.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <param name="predicate">Predicate function.</param>
        /// <returns>
        /// Either empty value or value that satisfies the specified predicate
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static T? Where<T>(this T? source, Func<T, bool> predicate)
            where T : struct
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return source is T value && predicate(value) ? value : default(T?);
        }

        /// <summary>
        /// Checks if value is empty or satisfies the specified predicate.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <param name="predicate">Predicate function.</param>
        /// <returns>
        /// <c>true</c> if value is either empty or satisfies the specified predicate, <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static bool All<T>(this Nullable<T> source, Func<T, bool> predicate)
            where T : struct
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return !source.HasValue || predicate(source.Value);
        }

        /// <summary>
        /// Checks if value is not empty.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <returns>
        /// <c>false</c> if value is empty, <c>true</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static bool Any<T>(this Nullable<T> source) where T : struct => source.HasValue;

        /// <summary>
        /// Checks if value is not empty and satisfies the specified predicate.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <param name="predicate">Predicate function.</param>
        /// <returns>
        /// <c>true</c> if value is not empty and satisfies the specified predicate, <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static bool Any<T>(this T? source, Func<T, bool> predicate)
            where T : struct
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return source.HasValue && predicate(source.Value);
        }

        /// <summary>
        /// Checks whether the source value is empty and if so returns new value careated using the specified factory
        /// function.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <param name="valueFactory">Factory function.</param>
        /// <returns>
        /// Either source value if not empty or value created using the specified factory function.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static T? Supply<T>(this T? source, Func<T> valueFactory)
            where T : struct
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }
            return source.HasValue ? source : new T?(valueFactory());
        }

        /// <summary>
        /// Gets the stored value or returns default value.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <param name="defaultValue">Default value to return instead of <c>default(T)</c>.</param>
        /// <returns>
        /// Either stored value or the default value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static T GetOrDefault<T>(this T? source, T defaultValue = default)
            where T : struct
            => source ?? defaultValue;

        /// <summary>
        /// Checks whether the value is empty.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <returns>
        /// <c>true</c> of value is empty, <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static bool IsEmpty<T>(this Nullable<T> source) where T : struct => !source.HasValue;

        /// <summary>
        /// Applies value to accumulator if value is present.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <param name="accumulator">Accumulator object.</param>
        /// <param name="fun">Function that applies value to the accumulator.</param>
        /// <returns>
        /// Either passed accumulator or result of applying value to the accumulator.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static TAccumulator Aggregate<TSource, TAccumulator>(this Nullable<TSource> source, TAccumulator accumulator, Func<TAccumulator, TSource, TAccumulator> fun)
            where TSource : struct
        {
            if (fun == null)
            {
                throw new ArgumentNullException(nameof(fun));
            }
            return source.HasValue ? fun(accumulator, source.Value) : accumulator;
        }

        /// <summary>
        /// Creates enumeration that conatins single value stored by the actual instance if present.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <returns>Enumeration that conatins single value stored by the actual instance if present.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static IEnumerable<T> ToEnumerable<T>(this T? source)
            where T : struct
            => source.ToArray();

        /// <summary>
        /// Creates list that conatins single value stored by the actual instance if present.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <returns>List that conatins single value stored by the actual instance if present.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static List<T> ToList<T>(this T? source)
            where T : struct
            => source is T value
                ? new() { value }
                : new();

        /// <summary>
        /// Creates array that conatins single value stored by the actual instance if present.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <returns>Array that conatins single value stored by the actual instance if present.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static T[] ToArray<T>(this T? source)
            where T : struct
            => source.HasValue
                ? new T[] { source.Value }
#if NET6_0_OR_GREATER
                : Array.Empty<T>();
#else
                : new T[0];
#endif

        /// <summary>
        /// If both value is not empty applies their stored values to <paramref name="resultSelector" />.
        /// </summary>
        /// <param name="first">First value.</param>
        /// <param name="second">Second value.</param>
        /// <param name="resultSelector">Function that creates result from values.</param>
        /// <returns>
        /// Either empty value or result of applying values to <paramref name="resultSelector" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static TResult? Zip<TFirst, TSecond, TResult>(this TFirst? first, TSecond? second, Func<TFirst, TSecond, TResult> resultSelector)
            where TFirst : struct
            where TSecond : struct
            where TResult : struct
        {
            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }
            if (first.HasValue && second.HasValue)
            {
                return new TResult?(resultSelector(first.Value, second.Value));
            }
            return default;
        }

        /// <summary>
        /// Creates tupled value if both specified values are not empty.
        /// </summary>
        /// <param name="first">First value.</param>
        /// <param name="second">Second value.</param>
        /// <returns>
        /// Either empty value or value containing tupled representation of source values.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static Nullable<(TFirst, TSecond)> Zip<TFirst, TSecond>(this Nullable<TFirst> first, Nullable<TSecond> second)
            where TFirst : struct
            where TSecond : struct
            => first.Zip(second, (a, b) => (a, b));

        /// <summary>
        /// Attempts to get stored value.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <param name="value">Variable to store stored value if present.</param>
        /// <returns>
        /// <c>true</c> if source value was not empty and its stored value has been written to
        /// <paramref name="value" />, <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static bool TryGetValue<T>(this Nullable<T> source, out T value)
            where T : struct
        {
            if (source.HasValue)
            {
                value = source.Value;
                return true;
            }
            value = default;
            return false;
        }
    }
}
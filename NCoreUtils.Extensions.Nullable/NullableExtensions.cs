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
        /// <param name="mapper">Conversion function.</param>
        /// <returns>
        /// Either empty value of the target type or value containing result of the conversion.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static Nullable<TResult> Map<TSource, TResult>(this Nullable<TSource> source, Func<TSource, TResult> mapper)
            where TSource : struct
            where TResult : struct
        {
            if (source.HasValue)
            {
                return new Nullable<TResult>(mapper(source.Value));
            }
            return new Nullable<TResult>();
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
        public static Nullable<TResult> Bind<TSource, TResult>(this Nullable<TSource> source, Func<TSource, Nullable<TResult>> binder)
            where TSource : struct
            where TResult : struct
        {
            if (source.HasValue)
            {
                return binder(source.Value);
            }
            return new Nullable<TResult>();
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
        public static Nullable<T> Where<T>(this Nullable<T> source, Func<T, bool> predicate)
            where T : struct
        {
            if (source.HasValue && predicate(source.Value))
            {
                return source;
            }
            return new Nullable<T>();
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
            => !source.HasValue || predicate(source.Value);

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
        public static bool Any<T>(this Nullable<T> source, Func<T, bool> predicate)
            where T : struct
            => source.HasValue && predicate(source.Value);

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
        public static Nullable<T> Supply<T>(this Nullable<T> source, Func<T> valueFactory)
            where T : struct
            => source.HasValue ? source : new Nullable<T>(valueFactory());

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
        public static T GetOrDefault<T>(this Nullable<T> source, T defaultValue = default(T))
            where T : struct
            => source.HasValue ? source.Value : defaultValue;

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
            => source.HasValue ? accumulator : fun(accumulator, source.Value);

        /// <summary>
        /// Creates enumeration that conatins single value stored by the actual instance if present.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <returns>Enumeration that conatins single value stored by the actual instance if present.</returns>
        [DebuggerStepThrough]
        public static IEnumerable<T> ToEnumerable<T>(this Nullable<T> source)
            where T : struct
        {
            if (source.HasValue)
            {
                yield return source.Value;
            }
        }

        /// <summary>
        /// Creates list that conatins single value stored by the actual instance if present.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <returns>List that conatins single value stored by the actual instance if present.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static List<T> ToList<T>(this Nullable<T> source)
            where T : struct
        {
            var result = new List<T>(1);
            if (source.HasValue)
            {
                result.Add(source.Value);
            }
            return result;
        }

        /// <summary>
        /// Creates array that conatins single value stored by the actual instance if present.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <returns>Array that conatins single value stored by the actual instance if present.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static T[] ToArray<T>(this Nullable<T> source)
            where T : struct
            => source.HasValue ? new T[0] : new T[] { source.Value };

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
        public static Nullable<TResult> Zip<TFirst, TSecond, TResult>(this Nullable<TFirst> first, Nullable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
            where TFirst : struct
            where TSecond : struct
            where TResult : struct
        {
            if (first.HasValue && second.HasValue)
            {
                return new Nullable<TResult>(resultSelector(first.Value, second.Value));
            }
            return new Nullable<TResult>();
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
            value = default(T);
            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NCoreUtils
{
    /// <summary>
    /// Defines default runtime assertions.
    /// </summary>
    static class RuntimeAssert
    {
        /// <summary>
        /// Represents precompiled not-null asserition.
        /// </summary>
        public interface INotNullAssertion<T>
        {
            /// <summary>
            /// Throws if specified argument is null.
            /// </summary>
            /// <param name="obj">Argument value.</param>
            /// <param name="name">Argument name.</param>
            void ThrowIfNull(T obj, string name);
        }
        sealed class ValueTypeNotNullAssertion<T> : INotNullAssertion<T> where T : struct
        {
            public void ThrowIfNull(T obj, string name) { }
        }
        sealed class ReferenceTypeNotNullAssertion<T> : INotNullAssertion<T> where T : class
        {
            public void ThrowIfNull(T obj, string name)
            {
                if (null == obj)
                {
                    throw new ArgumentNullException(name);
                }
            }
        }
        sealed class NonNullAssertions<T>
        {
            public static readonly INotNullAssertion<T> SharedAssertion;

            [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026",
                Justification = "Type is bound by caller.")]
            static NonNullAssertions()
            {
                var assertionType = typeof(T).IsValueType ? typeof(ValueTypeNotNullAssertion<>) : typeof(ReferenceTypeNotNullAssertion<>);
                SharedAssertion = (INotNullAssertion<T>)Activator.CreateInstance(assertionType.MakeGenericType(typeof(T)))!;
            }
        }

        /// <summary>
        /// Throws if specified argument is null.
        /// </summary>
        /// <param name="obj">Argument value.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void ArgumentNotNull<T>(T obj, string name) => NonNullAssertions<T>.SharedAssertion.ThrowIfNull(obj, name);

        /// <summary>
        /// Throws if specified argument value is not equal to <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void Equals<T>(T argumentValue, T checkValue, string name) where T : IEquatable<T>
        {
            if (!argumentValue.Equals(checkValue))
            {
                throw new ArgumentException($"{name} must be equal to {checkValue}", name);
            }
        }

        /// <summary>
        /// Throws if specified argument value is not equal to <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="equalityComparer">Equality comparer to use.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void Equals<T>(T argumentValue, T checkValue, IEqualityComparer<T> equalityComparer, string name)
        {
            if (!equalityComparer.Equals(argumentValue, checkValue))
            {
                throw new ArgumentException($"{name} must be equal to {checkValue}", name);
            }
        }

        /// <summary>
        /// Throws if specified argument value is not equal to <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="equalityComparer">Equality comparer to use.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void Equals<T>(T argumentValue, T checkValue, Func<T, T, bool> equalityComparer, string name)
        {
            if (!equalityComparer(argumentValue, checkValue))
            {
                throw new ArgumentException($"{name} must be equal to {checkValue}", name);
            }
        }

        /// <summary>
        /// Throws if specified arguments are not equal.
        /// </summary>
        /// <param name="a">First argument value.</param>
        /// <param name="b">Second argument value.</param>
        /// <param name="aName">First argument name.</param>
        /// <param name="bName">Second argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void Equals<T>(T a, T b, string aName, string bName) where T : IEquatable<T>
        {
            if (!a.Equals(b))
            {
                throw new InvalidOperationException($"{aName} must be equal to {bName}");
            }
        }

        /// <summary>
        /// Throws if specified arguments are not equal.
        /// </summary>
        /// <param name="a">First argument value.</param>
        /// <param name="b">Second argument value.</param>
        /// <param name="equalityComparer">Equality comparer to use.</param>
        /// <param name="aName">First argument name.</param>
        /// <param name="bName">Second argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void Equals<T>(T a, T b, IEqualityComparer<T> equalityComparer, string aName, string bName)
        {
            if (!equalityComparer.Equals(a, b))
            {
                throw new InvalidOperationException($"{aName} must be equal to {bName}");
            }
        }

        /// <summary>
        /// Throws if specified arguments are not equal.
        /// </summary>
        /// <param name="a">First argument value.</param>
        /// <param name="b">Second argument value.</param>
        /// <param name="equalityComparer">Equality comparer to use.</param>
        /// <param name="aName">First argument name.</param>
        /// <param name="bName">Second argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void Equals<T>(T a, T b, Func<T, T, bool> equalityComparer, string aName, string bName)
        {
            if (!equalityComparer(a, b))
            {
                throw new InvalidOperationException($"{aName} must be equal to {bName}");
            }
        }

        /// <summary>
        /// Throws if specified argument value is equal to <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void NotEquals<T>(T argumentValue, T checkValue, string name) where T : IEquatable<T>
        {
            if (argumentValue.Equals(checkValue))
            {
                throw new ArgumentException($"{name} must be equal to {checkValue}", name);
            }
        }

        /// <summary>
        /// Throws if specified argument value is equal to <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="equalityComparer">Equality comparer to use.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void NotEquals<T>(T argumentValue, T checkValue, IEqualityComparer<T> equalityComparer, string name)
        {
            if (equalityComparer.Equals(argumentValue, checkValue))
            {
                throw new ArgumentException($"{name} must be equal to {checkValue}", name);
            }
        }

        /// <summary>
        /// Throws if specified argument value is equal to <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="equalityComparer">Equality comparer to use.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void NotEquals<T>(T argumentValue, T checkValue, Func<T, T, bool> equalityComparer, string name)
        {
            if (equalityComparer(argumentValue, checkValue))
            {
                throw new ArgumentException($"{name} must be equal to {checkValue}", name);
            }
        }

        /// <summary>
        /// Throws if specified arguments are equal.
        /// </summary>
        /// <param name="a">First argument value.</param>
        /// <param name="b">Second argument value.</param>
        /// <param name="aName">First argument name.</param>
        /// <param name="bName">Second argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void NotEquals<T>(T a, T b, string aName, string bName) where T : IEquatable<T>
        {
            if (a.Equals(b))
            {
                throw new InvalidOperationException($"{aName} must not be equal to {bName}");
            }
        }

        /// <summary>
        /// Throws if specified arguments are equal.
        /// </summary>
        /// <param name="a">First argument value.</param>
        /// <param name="b">Second argument value.</param>
        /// <param name="equalityComparer">Equality comparer to use.</param>
        /// <param name="aName">First argument name.</param>
        /// <param name="bName">Second argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void NotEquals<T>(T a, T b, IEqualityComparer<T> equalityComparer, string aName, string bName)
        {
            if (equalityComparer.Equals(a, b))
            {
                throw new InvalidOperationException($"{aName} must not be equal to {bName}");
            }
        }

        /// <summary>
        /// Throws if specified arguments are equal.
        /// </summary>
        /// <param name="a">First argument value.</param>
        /// <param name="b">Second argument value.</param>
        /// <param name="equalityComparer">Equality comparer to use.</param>
        /// <param name="aName">First argument name.</param>
        /// <param name="bName">Second argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void NotEquals<T>(T a, T b, Func<T, T, bool> equalityComparer, string aName, string bName)
        {
            if (equalityComparer(a, b))
            {
                throw new InvalidOperationException($"{aName} must not be equal to {bName}");
            }
        }

        /// <summary>
        /// Throws if the specified argument value is less than or equal to the <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">First argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void Greater<T>(T argumentValue, T checkValue, string name) where T : IComparable<T>
        {
            if (0 >= argumentValue.CompareTo(checkValue))
            {
                throw new ArgumentException($"{name} must be greater than {checkValue}");
            }
        }

        /// <summary>
        /// Throws if the specified argument value is less than or equal to the <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="comparer">Comparer to use.</param>
        /// <param name="name">Argument name.</param>
        public static void Greater<T>(T argumentValue, T checkValue, IComparer<T> comparer, string name)
        {
            if (0 >= comparer.Compare(argumentValue, checkValue))
            {
                throw new ArgumentException($"{name} must be greater than {checkValue}");
            }
        }

        /// <summary>
        /// Throws if the specified argument value is less than or equal to the <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="comparer">Comparer to use.</param>
        /// <param name="name">Argument name.</param>
        public static void Greater<T>(T argumentValue, T checkValue, Func<T, T, int> comparer, string name)
        {
            if (0 >= comparer(argumentValue, checkValue))
            {
                throw new ArgumentException($"{name} must be greater than {checkValue}", name);
            }
        }

        /// <summary>
        /// Throws if the specified argument value is less than the <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void GreaterOrEquals<T>(T argumentValue, T checkValue, string name) where T : IComparable<T>
        {
            if (0 > argumentValue.CompareTo(checkValue))
            {
                throw new ArgumentException($"{name} must be greater than or equal to {checkValue}", name);
            }
        }

        /// <summary>
        /// Throws if the specified argument value is less than the <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="comparer">Comparer to use.</param>
        /// <param name="name">Argument name.</param>
        public static void GreaterOrEquals<T>(T argumentValue, T checkValue, IComparer<T> comparer, string name)
        {
            if (0 > comparer.Compare(argumentValue, checkValue))
            {
                throw new ArgumentException($"{name} must be greater than or equal to {checkValue}", name);
            }
        }

        /// <summary>
        /// Throws if the specified argument value is less than the <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="comparer">Comparer to use.</param>
        /// <param name="name">Argument name.</param>
        public static void GreaterOrEquals<T>(T argumentValue, T checkValue, Func<T, T, int> comparer, string name)
        {
            if (0 > comparer(argumentValue, checkValue))
            {
                throw new ArgumentException($"{name} must be greater than or equal to {checkValue}", name);
            }
        }

        /// <summary>
        /// Throws if the specified argument value is greater than or equal to the <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void Less<T>(T argumentValue, T checkValue, string name) where T : IComparable<T>
        {
            if (0 <= argumentValue.CompareTo(checkValue))
            {
                throw new ArgumentException($"{name} must be less than {checkValue}", name);
            }
        }

        /// <summary>
        /// Throws if the specified argument value is greater than or equal to the <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="comparer">Comparer to use.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void Less<T>(T argumentValue, T checkValue, IComparer<T> comparer, string name)
        {
            if (0 <= comparer.Compare(argumentValue, checkValue))
            {
                throw new ArgumentException($"{name} must be less than {checkValue}", name);
            }
        }

        /// <summary>
        /// Throws if the specified argument value is greater than or equal to the <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="comparer">Comparer to use.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void Less<T>(T argumentValue, T checkValue, Func<T, T, int> comparer, string name)
        {
            if (0 <= comparer(argumentValue, checkValue))
            {
                throw new ArgumentException($"{name} must be less than {checkValue}", name);
            }
        }

        /// <summary>
        /// Throws if the specified argument value is greater than the <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void LessOrEquals<T>(T argumentValue, T checkValue, string name) where T : IComparable<T>
        {
            if (0 < argumentValue.CompareTo(checkValue))
            {
                throw new ArgumentException($"{name} must be less than {checkValue}", name);
            }
        }

        /// <summary>
        /// Throws if the specified argument value is greater than the <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="comparer">Comparer to use.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void LessOrEquals<T>(T argumentValue, T checkValue, IComparer<T> comparer, string name)
        {
            if (0 < comparer.Compare(argumentValue, checkValue))
            {
                throw new ArgumentException($"{name} must be less than {checkValue}", name);
            }
        }

        /// <summary>
        /// Throws if the specified argument value is greater than the <paramref name="checkValue" />.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="checkValue">Check value.</param>
        /// <param name="comparer">Comparer to use.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void LessOrEquals<T>(T argumentValue, T checkValue, Func<T, T, int> comparer, string name)
        {
            if (0 < comparer(argumentValue, checkValue))
            {
                throw new ArgumentException($"{name} must be less than {checkValue}", name);
            }
        }

        /// <summary>
        /// Throws if the specified argument is not within a range.
        /// </summary>
        /// <param name="value">Argument value.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Max.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void IndexInRange(int value, int min, int max, string name)
        {
            if (value < min || value > max)
            {
                throw new IndexOutOfRangeException(string.Format("{0} must be in range [{1}, {2}] but is {3}", name, min, max, value));
            }
        }

        /// <summary>
        /// Throws if the specified argument is not within a range.
        /// </summary>
        /// <param name="value">Argument value.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Max.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void ArgumentInRange<T>(T value, T min, T max, string name) where T : IComparable<T>
        {
            if (0 > value.CompareTo(min) || 0 < value.CompareTo(max))
            {
                throw new ArgumentOutOfRangeException($"{name} must be in range [{min}, {max}] but has value {value}", name);
            }
        }

        /// <summary>
        /// Throws if the specified argument is not within a range.
        /// </summary>
        /// <param name="value">Argument value.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Max.</param>
        /// <param name="comparer">Comparer to use.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void ArgumentInRange<T>(T value, T min, T max, IComparer<T> comparer, string name)
        {
            if (0 > comparer.Compare(value, min) || 0 < comparer.Compare(value, max))
            {
                throw new ArgumentOutOfRangeException($"{name} must be in range [{min}, {max}] but has value {value}", name);
            }
        }

        /// <summary>
        /// Throws if the specified argument is not within a range.
        /// </summary>
        /// <param name="value">Argument value.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Max.</param>
        /// <param name="comparer">Comparer to use.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void ArgumentInRange<T>(T value, T min, T max, Func<T, T, int> comparer, string name)
        {
            if (0 > comparer(value, min) || 0 < comparer(value, max))
            {
                throw new ArgumentOutOfRangeException($"{name} must be in range [{min}, {max}] but has value {value}", name);
            }
        }

        /// <summary>
        /// Throws if the specified array is empty.
        /// </summary>
        /// <param name="array">Array to check.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void ArgumentNotEmpty<T>(T[] array, string name)
        {
            if (0 == array.Length)
            {
                throw new ArgumentException($"{name} must not be empty.", name);
            }
        }

        /// <summary>
        /// Throws if the specified container is empty.
        /// </summary>
        /// <param name="collection">Container to check.</param>
        /// <param name="name">Argument name.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static void ArgumentNotEmpty<T>(IReadOnlyCollection<T> collection, string name)
        {
            if (0 == collection.Count)
            {
                throw new ArgumentException($"{name} must not be empty.", name);
            }
        }
    }
}
using System;
using System.Collections.Generic;

namespace NCoreUtils
{
    public static class SpanExtensions
    {
        #region specializations

        public static bool IsSame(this in ReadOnlySpan<byte> first, in ReadOnlySpan<byte> second)
        {
            if (first.Length != second.Length)
            {
                return false;
            }
            for (var i = 0; i < first.Length; ++i)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsSame(this in ReadOnlySpan<byte> first, in Span<byte> second)
        {
            if (first.Length != second.Length)
            {
                return false;
            }
            for (var i = 0; i < first.Length; ++i)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsSame(this in Span<byte> first, in ReadOnlySpan<byte> second)
        {
            if (first.Length != second.Length)
            {
                return false;
            }
            for (var i = 0; i < first.Length; ++i)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsSame(this in Span<byte> first, in Span<byte> second)
        {
            if (first.Length != second.Length)
            {
                return false;
            }
            for (var i = 0; i < first.Length; ++i)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        public static bool IsSame<T>(this in ReadOnlySpan<T> first, in ReadOnlySpan<T> second, IEqualityComparer<T>? equalityComparer = default)
            where T : unmanaged
        {
            if (first.Length != second.Length)
            {
                return false;
            }
            var eq = equalityComparer ?? EqualityComparer<T>.Default;
            for (var i = 0; i < first.Length; ++i)
            {
                if (!eq.Equals(first[i], second[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsSame<T>(this in ReadOnlySpan<T> first, in Span<T> second, IEqualityComparer<T>? equalityComparer = default)
            where T : unmanaged
        {
            if (first.Length != second.Length)
            {
                return false;
            }
            var eq = equalityComparer ?? EqualityComparer<T>.Default;
            for (var i = 0; i < first.Length; ++i)
            {
                if (!eq.Equals(first[i], second[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsSame<T>(this in Span<T> first, in ReadOnlySpan<T> second, IEqualityComparer<T>? equalityComparer = default)
            where T : unmanaged
        {
            if (first.Length != second.Length)
            {
                return false;
            }
            var eq = equalityComparer ?? EqualityComparer<T>.Default;
            for (var i = 0; i < first.Length; ++i)
            {
                if (!eq.Equals(first[i], second[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsSame<T>(this in Span<T> first, in Span<T> second, IEqualityComparer<T>? equalityComparer = default)
            where T : unmanaged
        {
            if (first.Length != second.Length)
            {
                return false;
            }
            var eq = equalityComparer ?? EqualityComparer<T>.Default;
            for (var i = 0; i < first.Length; ++i)
            {
                if (!eq.Equals(first[i], second[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
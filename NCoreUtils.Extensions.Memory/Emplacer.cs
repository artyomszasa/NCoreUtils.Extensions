using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NCoreUtils.Memory;

namespace NCoreUtils
{
    public static class Emplacer
    {
        private static readonly ConcurrentDictionary<Type, object> _emplacers = new(new Dictionary<Type, object>
        {
            { typeof(sbyte), Int8Emplacer.Instance },
            { typeof(short), Int16Emplacer.Instance },
            { typeof(int), Int32Emplacer.Instance },
            { typeof(long), Int64Emplacer.Instance },
            { typeof(byte), UInt8Emplacer.Instance },
            { typeof(ushort), UInt16Emplacer.Instance },
            { typeof(uint), UInt32Emplacer.Instance },
            { typeof(ulong), UInt64Emplacer.Instance },
            { typeof(float), SingleEmplacer.Default },
            { typeof(double), DoubleEmplacer.Default },
            { typeof(char), CharEmplacer.Instance },
            { typeof(string), StringEmplacer.Instance }
        });

        private static char I(int value) => unchecked((char)('0' + value));

        #region char

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryEmplaceInternal(char value, Span<char> span, out int total)
        {
            total = 1;
            if (span.Length < 1)
            {
                return false;
            }
            span[0] = value;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Emplace(char value, Span<char> span)
        {
            if (TryEmplaceInternal(value, span, out int length))
            {
                return length;
            }
            throw new InsufficientBufferSizeException(span, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryEmplace(char value, Span<char> span, out int used)
        {
            if (TryEmplaceInternal(value, span, out int length))
            {
                used = length;
                return true;
            }
            used = default;
            return false;
        }

        #endregion

        #region sbyte

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Emplace(sbyte value, Span<char> span)
            => Emplace((int)value, span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryEmplace(sbyte value, Span<char> span, out int used)
            => TryEmplace((int)value, span, out used);

        #endregion

        #region short

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Emplace(short value, Span<char> span)
            => Emplace((int)value, span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryEmplace(short value, Span<char> span, out int used)
            => TryEmplace((int)value, span, out used);

        #endregion

        #region int

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryEmplaceInternal(int value, Span<char> span, out int total)
            => StringUtils.TryFormatInt32(value, span, out total);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Emplace(int value, Span<char> span)
        {
            if (TryEmplaceInternal(value, span, out int length))
            {
                return length;
            }
            throw new InsufficientBufferSizeException(span, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryEmplace(int value, Span<char> span, out int used)
        {
            if (TryEmplaceInternal(value, span, out int length))
            {
                used = length;
                return true;
            }
            used = default;
            return false;
        }

        #endregion

        #region long

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryEmplaceInternal(long value, Span<char> span, out int total)
            => StringUtils.TryFormatInt64(value, span, out total);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Emplace(long value, Span<char> span)
        {
            if (TryEmplaceInternal(value, span, out int length))
            {
                return length;
            }
            throw new InsufficientBufferSizeException(span, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryEmplace(long value, Span<char> span, out int used)
        {
            if (TryEmplaceInternal(value, span, out int length))
            {
                used = length;
                return true;
            }
            used = default;
            return false;
        }

        #endregion

        #region float

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool TryEmplaceInternal(float value, Span<char> span, int maxPrecision, string decimalSeparator, out int total)
        {
            if (0.0f == value)
            {
                return TryEmplaceInternal('0', span, out total);
            }
            Span<char> fbuffer = stackalloc char[maxPrecision];
            var uvalue = Math.Abs(value);
            // intgeral part
            var ivalue = (int)value;
            var isNegative = ivalue < 0 ? 1 : 0;
            var uivalue = Math.Abs(ivalue);
            // floating part
            var fvalue = uvalue - uivalue;
            // intgeral part length...
            var ilength = StringUtils.GetDigitCount(unchecked((ulong)uivalue)) + isNegative;
            // stringify floating part locally to get value...
            var flength = 0;
            var flast = maxPrecision - 1;
            while (flength < maxPrecision)
            {
                var v = fvalue * Math.Pow(10.0, flength + 1);
                if (0.0 == v % 10.0)
                {
                    break;
                }
                if (flength == flast)
                {
                    fbuffer[flength] = I((int)Math.Round(v) % 10);
                }
                else
                {
                    fbuffer[flength] = I((int)v % 10);
                }
                ++flength;
            }
            total = ilength + (flength == 0 ? 0 : flength + decimalSeparator.Length);
            if (span.Length < total)
            {
                return false;
            }
            Emplace(ivalue, span);
            if (flength > 0)
            {
#if NETFRAMEWORK
                decimalSeparator.AsSpan().CopyTo(span.Slice(ilength));
                fbuffer.Slice(0, flength).CopyTo(span.Slice(ilength + decimalSeparator.Length));
#else
                decimalSeparator.AsSpan().CopyTo(span[ilength..]);
                fbuffer[..flength].CopyTo(span[(ilength + decimalSeparator.Length)..]);
#endif
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Emplace(float value, Span<char> span, int maxPrecision, string decimalSeparator = ".")
        {
            if (TryEmplaceInternal(value, span, maxPrecision, decimalSeparator, out var length))
            {
                return length;
            }
            throw new InsufficientBufferSizeException(span, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryEmplace(float value, Span<char> span, int maxPrecision, string decimalSeparator, out int used)
        {
            if (TryEmplaceInternal(value, span, maxPrecision, decimalSeparator, out int length))
            {
                used = length;
                return true;
            }
            used = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryEmplace(float value, Span<char> span, int maxPrecision, out int used)
            => TryEmplace(value, span, maxPrecision, SingleEmplacer.DefaultDecimalSeparator, out used);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryEmplace(float value, Span<char> span, out int used)
            => TryEmplace(value, span, SingleEmplacer.DefaultMaxPrecision, SingleEmplacer.DefaultDecimalSeparator, out used);

        #endregion

        #region double

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool TryEmplaceInternal(double value, Span<char> span, int maxPrecision, string decimalSeparator, out int total)
        {
            if (0.0 == value)
            {
                return TryEmplaceInternal('0', span, out total);
            }
            Span<char> fbuffer = stackalloc char[maxPrecision];
            var uvalue = Math.Abs(value);
            // intgeral part
            var ivalue = (long)value;
            var isNegative = ivalue < 0L ? 1 : 0;
            var uivalue = Math.Abs(ivalue);
            // floating part
            var fvalue = uvalue - uivalue;
            // intgeral part length...
            var ilength = (int)Math.Floor(Math.Log10(uivalue)) + 1 + isNegative;
            // stringify floating part locally to get value...
            var flength = 0;
            var flast = maxPrecision - 1;
            while (flength < maxPrecision)
            {
                var v = fvalue * Math.Pow(10.0, flength + 1);
                if (0.0 == v % 10.0)
                {
                    break;
                }
                if (flength == flast)
                {
                    fbuffer[flength] = I((int)Math.Round(v) % 10);
                }
                else
                {
                    fbuffer[flength] = I((int)v % 10);
                }
                ++flength;
            }
            total = ilength + (flength == 0 ? 0 : flength + decimalSeparator.Length);
            if (span.Length < total)
            {
                return false;
            }
            Emplace(ivalue, span);
            if (flength > 0)
            {
#if NETFRAMEWORK
                decimalSeparator.AsSpan().CopyTo(span.Slice(ilength));
                fbuffer.Slice(0, flength).CopyTo(span.Slice(ilength + decimalSeparator.Length));
#else
                decimalSeparator.AsSpan().CopyTo(span[ilength..]);
                fbuffer[..flength].CopyTo(span[(ilength + decimalSeparator.Length)..]);
#endif
            }
            return true;

            static char I(int value) => (char)('0' + value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Emplace(double value, Span<char> span, int maxPrecision, string decimalSeparator = ".")
        {
            if (TryEmplaceInternal(value, span, maxPrecision, decimalSeparator, out var length))
            {
                return length;
            }
            throw new InsufficientBufferSizeException(span, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryEmplace(double value, Span<char> span, int maxPrecision, string decimalSeparator, out int used)
        {
            if (TryEmplaceInternal(value, span, maxPrecision, decimalSeparator, out int length))
            {
                used = length;
                return true;
            }
            used = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryEmplace(double value, Span<char> span, int maxPrecision, out int used)
            => TryEmplace(value, span, maxPrecision, DoubleEmplacer.DefaultDecimalSeparator, out used);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryEmplace(double value, Span<char> span, out int used)
            => TryEmplace(value, span, DoubleEmplacer.DefaultMaxPrecision, DoubleEmplacer.DefaultDecimalSeparator, out used);

        #endregion

        #region string

        public static bool TryEmplaceInternal(string? value, Span<char> span, out int total)
        {
            if (value is null)
            {
                total = 0;
                return true;
            }
            total = value.Length;
            if (value.Length > span.Length)
            {
                return false;
            }
            value.AsSpan().CopyTo(span);
            return true;
        }

        public static int Emplace(string? value, Span<char> span)
        {
            if (TryEmplaceInternal(value, span, out var length))
            {
                return length;
            }
            throw new InsufficientBufferSizeException(span, length);
        }

        public static bool TryEmplace(string? value, Span<char> span, out int used)
        {
            if (TryEmplaceInternal(value, span, out var length))
            {
                used = length;
                return true;
            }
            used = default;
            return false;
        }

        #endregion

        #region byte

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Emplace(byte value, Span<char> span)
            => Emplace((int)value, span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryEmplace(byte value, Span<char> span, out int used)
            => TryEmplace((int)value, span, out used);

        #endregion

        #region ushort

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Emplace(ushort value, Span<char> span)
            => Emplace((int)value, span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryEmplace(ushort value, Span<char> span, out int used)
            => TryEmplace((int)value, span, out used);

        #endregion

        #region uint

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Emplace(uint value, Span<char> span)
            => Emplace((long)value, span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryEmplace(uint value, Span<char> span, out int used)
            => TryEmplace((long)value, span, out used);


        #endregion

        #region ulong

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryEmplaceInternal(ulong value, Span<char> span, out int total)
            => StringUtils.TryFormatUInt64(value, span, out total);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Emplace(ulong value, Span<char> span)
        {
            if (TryEmplaceInternal(value, span, out int length))
            {
                return length;
            }
            throw new InsufficientBufferSizeException(span, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryEmplace(ulong value, Span<char> span, out int used)
        {
            if (TryEmplaceInternal(value, span, out int length))
            {
                used = length;
                return true;
            }
            used = default;
            return false;
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Emplace<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(T value, Span<char> span)
        {
            if (_emplacers.TryGetValue(typeof(T), out var boxed))
            {
                return ((IEmplacer<T>)boxed).Emplace(value, span);
            }
            return value switch
            {
                ISpanEmplaceable emplaceable => emplaceable.Emplace(span),
#pragma warning disable CS0618
                IEmplaceable<T> emplaceable => emplaceable.Emplace(span),
#pragma warning restore CS0618
#if NET6_0_OR_GREATER
                ISpanFormattable spanFormattable => spanFormattable.TryFormat(span, out var written, default, default)
                    ? written
                    : throw new InsufficientBufferSizeException(span),
#endif
                _ => DefaultEmplacer<T>.DoEmplace(value, span)
            };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryEmplace<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(T value, Span<char> span, out int used)
        {
            if (_emplacers.TryGetValue(typeof(T), out var boxed))
            {
                return ((IEmplacer<T>)boxed).TryEmplace(value, span, out used);
            }
            var success = value switch
            {
                ISpanEmplaceable emplaceable => emplaceable.TryEmplace(span, out used),
#pragma warning disable CS0618
                IEmplaceable<T> emplaceable => emplaceable.TryEmplace(span, out used),
#pragma warning restore CS0618
#if NET6_0_OR_GREATER
                ISpanFormattable spanFormattable => spanFormattable.TryFormat(span, out used, default, default),
#endif
                _ => DefaultEmplacer<T>.DoTryEmplace(value, span, out used)
            };
            return success;
        }

        [RequiresUnreferencedCode(W.DynamicallyCreatedEmplacer)]
#if NET7_0_OR_GREATER
        [RequiresDynamicCode(W.DynamicallyCreatedEmplacer)]
#endif
        private static IEmplacer<T> GetDefaultInternal<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>()
        {
            if (typeof(ISpanEmplaceable).IsAssignableFrom(typeof(T)))
            {
                return (IEmplacer<T>)Activator.CreateInstance(typeof(SpanEmplaceableEmplacer<>).MakeGenericType(typeof(T)), true)!;
            }
#pragma warning disable CS0618
            if (typeof(IEmplaceable<T>).IsAssignableFrom(typeof(T)))
            {
                return (IEmplacer<T>)Activator.CreateInstance(typeof(EmplaceableEmplacer<>).MakeGenericType(typeof(T)), true)!;
            }
#pragma warning restore CS0618
            return new DefaultEmplacer<T>();
        }

        [RequiresUnreferencedCode(W.DynamicallyCreatedEmplacer)]
#if NET7_0_OR_GREATER
        [RequiresDynamicCode(W.DynamicallyCreatedEmplacer)]
#endif
        public static IEmplacer<T> GetDefault<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>()
        {
            if (_emplacers.TryGetValue(typeof(T), out var boxed))
            {
                return (IEmplacer<T>)boxed;
            }
            var emplacer = GetDefaultInternal<T>();
            _emplacers.TryAdd(typeof(T), emplacer);
            return emplacer;
        }

        /// <summary>
        /// Populates string representation of the <paramref name="value" /> using
        /// <see cref="IEmplaceable{T}.TryGetEmplaceBufferSize(out int)" />,
        /// <see cref="IEmplaceable{T}.Emplace(Span{char})" /> and character buffer obtained either from the array pool
        /// specified by <paramref name="arrayPool" /> or the shared array pool.
        /// </summary>
        /// <param name="value">Instance to populate string representation of.</param>
        /// <param name="arrayPool">
        /// Array pool to obtain character buffer from. <see cref="ArrayPool{T}.Shared" /> used if no pool is supplied.
        /// </param>
        /// <typeparam name="T">Type of the object to be stringified.</typeparam>
        /// <returns>String representation of the object.</returns>
        public static string ToStringUsingArrayPool<T>(this T value, ArrayPool<char>? arrayPool = default)
            where T : ISpanExactEmplaceable
        {
            var minimumBufferSize = value.GetEmplaceBufferSize();
            var pool = arrayPool ?? ArrayPool<char>.Shared;
            var buffer = pool.Rent(minimumBufferSize);
            try
            {
                var size = value.Emplace(buffer);
                return new string(buffer, 0, size);
            }
            finally
            {
                pool.Return(buffer, clearArray: false);
            }
        }
    }
}
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NCoreUtils;

public static class StringUtils
{
    #region digits

    private static ReadOnlySpan<byte> Log2ToPow10 =>
    [
        1,  1,  1,  2,  2,  2,  3,  3,  3,  4,  4,  4,  4,  5,  5,  5,
        6,  6,  6,  7,  7,  7,  7,  8,  8,  8,  9,  9,  9,  10, 10, 10,
        10, 11, 11, 11, 12, 12, 12, 13, 13, 13, 13, 14, 14, 14, 15, 15,
        15, 16, 16, 16, 16, 17, 17, 17, 18, 18, 18, 19, 19, 19, 19, 20
    ];

    private static ReadOnlySpan<ulong> PowersOf10 =>
    [
        0, // unused entry to avoid needing to subtract
        0,
        10,
        100,
        1000,
        10000,
        100000,
        1000000,
        10000000,
        100000000,
        1000000000,
        10000000000,
        100000000000,
        1000000000000,
        10000000000000,
        100000000000000,
        1000000000000000,
        10000000000000000,
        100000000000000000,
        1000000000000000000,
        10000000000000000000,
    ];

#if NETFRAMEWORK || NETSTANDARD2_0 || NETSTANDARD2_1

    private static ReadOnlySpan<byte> Log2DeBruijn => // 32
    [
        00, 09, 01, 10, 13, 21, 02, 29,
        11, 14, 16, 18, 22, 25, 03, 30,
        08, 12, 20, 28, 15, 17, 24, 07,
        19, 27, 23, 06, 26, 05, 04, 31
    ];

    private static int Log2(ulong value)
    {
        uint hi = (uint)(value >> 32);

        if (hi == 0)
        {
            return Log2I((uint)value);
        }

        return 32 + Log2I(hi);

        static int Log2I(uint value)
        {
            value |= value >> 01;
            value |= value >> 02;
            value |= value >> 04;
            value |= value >> 08;
            value |= value >> 16;

            // uint.MaxValue >> 27 is always in range [0 - 31] so we use Unsafe.AddByteOffset to avoid bounds check
            return Unsafe.AddByteOffset(
                // Using deBruijn sequence, k=2, n=5 (2^5=32) : 0b_0000_0111_1100_0100_1010_1100_1101_1101u
                ref MemoryMarshal.GetReference(Log2DeBruijn),
                // uint|long -> IntPtr cast on 32-bit platforms does expensive overflow checks not needed here
                (IntPtr)(int)((value * 0x07C4ACDDu) >> 27));
        }
    }
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Log2(ulong value) => System.Numerics.BitOperations.Log2(value);
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetDigitCount(ulong value)
    {
        uint index = Unsafe.Add(ref MemoryMarshal.GetReference(Log2ToPow10), Log2(value));
        ulong powerOf10 = Unsafe.Add(ref MemoryMarshal.GetReference(PowersOf10), unchecked((int)index));
        bool lessThan = value < powerOf10;
        return (int)(index - Unsafe.As<bool, byte>(ref lessThan));
    }

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryEmplaceInternal(char value, Span<char> span, out int total)
    {
        total = 1;
        if (span.Length < 1)
        {
            return false;
        }
        MemoryMarshal.GetReference(span) = value;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryEmplaceInternal(string? value, Span<char> span, out int total)
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

    private static char I(int value) => unchecked((char)('0' + value));

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.NoInlining)]
#endif
    public static bool TryFormatInt32(int value, Span<char> span, out int total)
    {
        if (value == int.MinValue)
        {
            return TryEmplaceInternal("-2147483648", span, out total);
        }
        if (0 == value)
        {
            return TryEmplaceInternal('0', span, out total);
        }
        var isSigned = value < 0 ? 1 : 0;
        value = Math.Abs(value);
        total = GetDigitCount(unchecked((ulong)value)) + isSigned;
        if (span.Length < total)
        {
            return false;
        }
        ref var buffer = ref MemoryMarshal.GetReference(span);
        if (0 != isSigned)
        {
            buffer = '-';
        }
        ref var ptr = ref Unsafe.Add(ref buffer, total - 1);
        for (var offset = isSigned; value > 0; ++offset)
        {
            value = Math.DivRem(value, 10, out var part);
            ptr = I(part);
            ptr = ref Unsafe.Add(ref ptr, -1);
        }
        return true;
    }

    public static bool TryFormatInt64(long value, Span<char> span, out int total)
    {
        if (value == long.MinValue)
        {
            return TryEmplaceInternal("-9223372036854775808", span, out total);
        }
        if (0L == value)
        {
            return TryEmplaceInternal('0', span, out total);
        }
        var isSigned = value < 0L ? 1 : 0;
        value = Math.Abs(value);
        total = GetDigitCount(unchecked((ulong)value)) + isSigned;
        if (span.Length < total)
        {
            return false;
        }
        ref var buffer = ref MemoryMarshal.GetReference(span);
        if (0 != isSigned)
        {
            buffer = '-';
        }
        ref var ptr = ref Unsafe.Add(ref buffer, total - 1);
        for (var offset = isSigned; value > 0L; ++offset)
        {
            value = Math.DivRem(value, 10, out var part);
            ptr = I(unchecked((int)part));
            ptr = ref Unsafe.Add(ref ptr, -1);
        }
        return true;
    }

    // FIXME: optimize
    private static ulong DivRem(ulong a, ulong b, out ulong reminder)
    {
        reminder = a % b;
        return a / b;
    }

    public static bool TryFormatUInt64(ulong value, Span<char> span, out int total)
    {
        if (0L == value)
        {
            return TryEmplaceInternal('0', span, out total);
        }
        total = GetDigitCount(value);
        if (span.Length < total)
        {
            return false;
        }
        ref var ptr = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), total - 1);
        for (var offset = 0; value != 0; ++offset)
        {
            value = DivRem(value, 10L, out var part);
            ptr = I(unchecked((int)part));
            ptr = ref Unsafe.Add(ref ptr, -1);
        }
        return true;
    }
}
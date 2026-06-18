using System;
using System.Runtime.CompilerServices;

namespace NCoreUtils;

public static partial class Base64
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryDecodeStandardPadded(ReadOnlySpan<char> base64, Span<byte> buffer, out int written)
        => Convert.TryFromBase64Chars(base64, buffer, out written);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool TryDecodeStandardNotPadded(ReadOnlySpan<char> base64, Span<byte> buffer, out int written)
    {
        var base64Length = base64.Length;
        var paddedLength = (base64Length + 3) & ~3;
        if (base64.Length <= MaxStackCharBufferSize)
        {
            Span<char> padded = stackalloc char[paddedLength];
            return PadAndTryDecodeStandard(base64, base64Length, padded, buffer, out written);
        }
        if (base64.Length <= MaxPooledCharBufferSize)
        {
            using var padded = SpanOwner.Allocate<char>(paddedLength);
            return PadAndTryDecodeStandard(base64, base64Length, padded, buffer, out written);
        }
        var paddedArray = new char[paddedLength];
        return PadAndTryDecodeStandard(base64, base64Length, paddedArray.AsSpan(0, paddedLength), buffer, out written);

        static bool PadAndTryDecodeStandard(
            ReadOnlySpan<char> base64,
            int base64Length,
            scoped Span<char> padded,
            Span<byte> buffer,
            out int written)
        {
            base64.CopyTo(padded);
            padded[base64Length..].Fill('=');
            return TryDecodeStandardPadded(padded, buffer, out written);
        }
    }
}
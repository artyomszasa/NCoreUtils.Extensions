using System;
using System.Runtime.CompilerServices;

namespace NCoreUtils;

public static partial class Base64
{
    private static bool TryEncodeStandardNotPaddedCore(ReadOnlySpan<byte> data, Span<char> buffer, out int written)
    {
        var remainder = data.Length % 3;
        if (remainder == 0)
        {
            return TryEncodeStandardPaddedCore(data, buffer, out written);
        }
        var paddingSize = remainder switch
        {
            1 => 2,
            2 => 1,
            _ => 0
        };
        if (Convert.TryToBase64Chars(data, buffer, out var writtenWithPadding, Base64FormattingOptions.None))
        {
            written = writtenWithPadding - paddingSize;
            return true;
        }
        var paddedLength = (data.Length / 3 + 1) * 4;
        if (paddedLength <= MaxStackCharBufferSize)
        {
            Span<char> padded = stackalloc char[paddedLength];
            return EncodeStandardThenUnpad(data, padded, paddingSize, buffer, out written);
        }
        if (paddedLength <= MaxPooledCharBufferSize)
        {
            using var padded = SpanOwner.Allocate<char>(paddedLength);
            return EncodeStandardThenUnpad(data, padded, paddingSize, buffer, out written);
        }
        var paddedArray = new char[paddedLength];
        return EncodeStandardThenUnpad(data, paddedArray.AsSpan(), paddingSize, buffer, out written);

        static bool EncodeStandardThenUnpad(ReadOnlySpan<byte> data, Span<char> padded, int paddingSize, Span<char> buffer, out int written)
        {
            if (!Convert.TryToBase64Chars(data, padded, out var writtenWithPadding, Base64FormattingOptions.None))
            {
                throw new InvalidOperationException("Unexpected encoding failure.");
            }
            padded[..buffer.Length].CopyTo(buffer);
            written = writtenWithPadding - paddingSize;
            return true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryEncodeStandardPaddedCore(ReadOnlySpan<byte> data, Span<char> buffer, out int written)
        => Convert.TryToBase64Chars(data, buffer, out written, Base64FormattingOptions.None);
}
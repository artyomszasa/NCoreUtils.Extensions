using System;
using System.Buffers.Text;
using System.Runtime.CompilerServices;

namespace NCoreUtils;

public partial class Base64
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryEncodeUrlNotPaddedCore(ReadOnlySpan<byte> data, Span<char> buffer, out int written)
        => Base64Url.TryEncodeToChars(data, buffer, out written);

    private static bool TryEncodeUrlPaddedCore(ReadOnlySpan<byte> data, Span<char> buffer, out int written)
    {
        if (TryEncodeUrlNotPaddedCore(data, buffer, out var writtenWithoutPadding))
        {
            var remainder = data.Length % 3;
            switch (remainder)
            {
                case 1:
                    if (buffer.Length < writtenWithoutPadding + 2)
                    {
                        written = default;
                        return false;
                    }
                    buffer[writtenWithoutPadding++] = '=';
                    buffer[writtenWithoutPadding++] = '=';
                    written = writtenWithoutPadding;
                    return true;
                case 2:
                    if (buffer.Length < writtenWithoutPadding + 1)
                    {
                        written = default;
                        return false;
                    }
                    buffer[writtenWithoutPadding++] = '=';
                    written = writtenWithoutPadding;
                    return true;
                default:
                    written = writtenWithoutPadding;
                    return true;
            }
        }
        written = default;
        return false;
    }
}
using System;
using System.Buffers.Text;
using System.Runtime.CompilerServices;

namespace NCoreUtils;

public static partial class Base64
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryDecodeUrlNotPadded(ReadOnlySpan<char> base64, Span<byte> buffer, out int written)
        => Base64Url.TryDecodeFromChars(base64, buffer, out written);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryDecodeUrlPadded(ReadOnlySpan<char> base64, uint paddingSize, Span<byte> buffer, out int written)
        => TryDecodeUrlNotPadded(base64[..^unchecked((int)paddingSize)], buffer, out written);
}
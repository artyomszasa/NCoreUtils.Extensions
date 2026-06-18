using System;
using System.Buffers.Text;
using System.Runtime.CompilerServices;

namespace NCoreUtils;

public static partial class Base64
{
    private static void MapUrlToStandard(Span<char> data)
    {
#if NET8_0_OR_GREATER
        data.Replace('-', '+');
        data.Replace('_', '/');
#else
        for (var i = 0; i < data.Length; ++i)
        {
            var ch = data[i];
            switch (ch)
            {
                case '-':
                    data[i] = '+';
                    break;
                case '_':
                    data[i] = '/';
                    break;
            }
        }
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryDecodeUrlNotPadded(ReadOnlySpan<char> base64, Span<byte> buffer, out int written)
    {
        // NOTE: netstandard and net prior 10 --> fallback to standard base64
        var base64Length = base64.Length;
        var paddedLength = (base64Length + 3) & ~3;
        var paddingSize = paddedLength - base64Length;
        if (base64.Length <= MaxStackCharBufferSize)
        {
            Span<char> padded = stackalloc char[paddedLength];
            return PadThenMapAndTryDecodeUrl(base64, paddingSize, padded, buffer, out written);
        }
        if (base64.Length <= MaxPooledCharBufferSize)
        {
            using var padded = SpanOwner.Allocate<char>(paddedLength);
            return PadThenMapAndTryDecodeUrl(base64, paddingSize, padded, buffer, out written);
        }
        var paddedArray = new char[paddedLength];
        return PadThenMapAndTryDecodeUrl(base64, paddingSize, paddedArray.AsSpan(0, paddedLength), buffer, out written);

        static bool PadThenMapAndTryDecodeUrl(ReadOnlySpan<char> base64, int paddingSize, Span<char> padded, Span<byte> buffer, out int written)
        {
            base64.CopyTo(padded);
            padded[^paddingSize..].Fill('=');
            MapUrlToStandard(padded);
            return TryDecodeStandardPadded(padded, buffer, out written);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryDecodeUrlPadded(ReadOnlySpan<char> base64, uint paddingSize, Span<byte> buffer, out int written)
    {
        var base64Length = base64.Length;
        if (base64Length <= MaxStackCharBufferSize)
        {
            Span<char> mapBuffer = stackalloc char[base64Length];
            return MapAndTryDecodeUrl(base64, mapBuffer, buffer, out written);
        }
        if (base64Length <= MaxPooledCharBufferSize)
        {
            using var mapBuffer = SpanOwner.Allocate<char>(base64Length);
            return MapAndTryDecodeUrl(base64, mapBuffer, buffer, out written);
        }
        var mapArray = new char[base64Length];
        return MapAndTryDecodeUrl(base64, mapArray.AsSpan(0, base64Length), buffer, out written);

        static bool MapAndTryDecodeUrl(ReadOnlySpan<char> base64, Span<char> padded, Span<byte> buffer, out int written)
        {
            base64.CopyTo(padded);
            MapUrlToStandard(padded);
            return TryDecodeStandardPadded(padded, buffer, out written);
        }
    }
}
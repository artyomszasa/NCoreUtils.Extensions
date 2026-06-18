using System;
using System.Buffers.Text;
using System.Runtime.CompilerServices;

namespace NCoreUtils;

public partial class Base64
{
    private static void MapStandardToUrl(Span<char> data)
    {
#if NET8_0_OR_GREATER
        data.Replace('+', '-');
        data.Replace('/', '_');
#else
        for (var i = 0; i < data.Length; ++i)
        {
            var ch = data[i];
            switch (ch)
            {
                case '+':
                    data[i] = '-';
                    break;
                case '/':
                    data[i] = '_';
                    break;
            }
        }
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryEncodeUrlNotPaddedCore(ReadOnlySpan<byte> data, Span<char> buffer, out int written)
    {
        if (TryEncodeStandardNotPaddedCore(data, buffer, out written))
        {
            MapStandardToUrl(buffer[..written]);
            return true;
        }
        return false;
    }

    private static bool TryEncodeUrlPaddedCore(ReadOnlySpan<byte> data, Span<char> buffer, out int written)
    {
        if (TryEncodeStandardPaddedCore(data, buffer, out written))
        {
            MapStandardToUrl(buffer[..written]);
            return true;
        }
        return false;
    }
}
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NCoreUtils.Colors;

internal static class RgbFormat
{
    private static bool TryFormatRgbNoParen(byte r, byte g, byte b, Span<char> destination, out int charsWritten)
    {
        var offset = 4; // "rgb(".Length
        if (destination.Length < 5)
        {
            goto fail;
        }
        "rgb(".AsSpan().CopyTo(destination);
        if (!ByteFormat.TryFormat(r, destination[4..], out var used)) { goto fail; }
        offset += used;
        if (destination.Length == offset) { goto fail; }
        destination[offset++] = ',';
        if (!ByteFormat.TryFormat(g, destination[offset..], out used)) { goto fail; }
        offset += used;
        if (destination.Length == offset) { goto fail; }
        destination[offset++] = ',';
        if (!ByteFormat.TryFormat(b, destination[offset..], out used)) { goto fail; }
        charsWritten = offset + used;
        return true;
    fail:
        charsWritten = default;
        return false;
    }

    public static bool TryFormatRgb(byte r, byte g, byte b, Span<char> destination, out int charsWritten)
    {
        if (!TryFormatRgbNoParen(r, g, b, destination, out var used)) { goto fail; }
        var offset = used;
        if (destination.Length == offset) { goto fail; }
        destination[offset++] = ')';
        charsWritten = offset;
        return true;
    fail:
        charsWritten = default;
        return false;
    }

    [MethodImpl(Opt.Inline)]
    private static (uint Quotent, uint Reminder) DivRem(uint a, uint b)
    {
#if NET6_0_OR_GREATER
        return Math.DivRem(a, b);
#else
        var reminder = Math.DivRem(unchecked((int)a), unchecked((int)b), out var quotent);
        return (unchecked((uint)quotent), unchecked((uint)reminder));
#endif
    }

    private static bool TryFormatAlpha(byte alpha, Span<char> destination, out int charsWritten)
    {
        if (alpha == byte.MaxValue)
        {
            if (destination.Length < 1)
            {
                charsWritten = default;
                return false;
            }
            destination[0] = '1';
            charsWritten = 1;
            return true;
        }
        if (alpha == 0)
        {
            if (destination.Length < 1)
            {
                charsWritten = default;
                return false;
            }
            destination[0] = '0';
            charsWritten = 1;
            return true;
        }
        var fractionalPart = (uint)Math.Round((float)alpha / byte.MaxValue * 100.0f);
        var (n1, n0) = DivRem(fractionalPart, 10);
        if (n0 == 0)
        {
            if (destination.Length < 3)
            {
                charsWritten = default;
                return false;
            }
            destination[0] = '0';
            destination[1] = '.';
            destination[2] = unchecked((char)(n1 + '0'));
            charsWritten = 3;
            return true;
        }
        if (destination.Length < 4)
        {
            charsWritten = default;
            return false;
        }
        destination[0] = '0';
        destination[1] = '.';
        destination[2] = unchecked((char)(n1 + '0'));
        destination[3] = unchecked((char)(n0 + '0'));
        charsWritten = 4;
        return true;

    }

    public static bool TryFormatRgba(byte r, byte g, byte b, byte alpha, Span<char> destination, out int charsWritten)
    {
        if (!TryFormatRgbNoParen(r, g, b, destination, out var used)) { goto fail; }
        var offset = used;
        if (destination.Length == offset) { goto fail; }
        destination[offset++] = ',';
        if (!TryFormatAlpha(alpha, destination[offset..], out used)) { goto fail; }
        offset += used;
        if (destination.Length == offset) { goto fail; }
        destination[offset++] = ')';
        charsWritten = offset;
        return true;
    fail:
        charsWritten = default;
        return false;
    }

    public static bool TryFormatRgb(byte r, byte g, byte b, byte alpha, Span<char> destination, out int charsWritten)
        => alpha == byte.MaxValue
            ? TryFormatRgb(r, g, b, destination, out charsWritten)
            : TryFormatRgba(r, g, b, alpha, destination, out charsWritten);

    public static bool TryFormatHex(byte r, byte g, byte b, Span<char> destination, out int charsWritten)
    {
        if (destination.Length < 7)
        {
            charsWritten = default;
            return false;
        }
        destination[0] = '#';
        ByteFormat.EmplaceHexNoCheck(ref Unsafe.Add(ref MemoryMarshal.GetReference(destination), 1), r, g, b);
        charsWritten = 7;
        return true;
    }

    public static bool TryFormatHexa(byte r, byte g, byte b, byte alpha, Span<char> destination, out int charsWritten)
    {
        if (destination.Length < 9)
        {
            charsWritten = default;
            return false;
        }
        destination[0] = '#';
        ByteFormat.EmplaceHexNoCheck(ref Unsafe.Add(ref MemoryMarshal.GetReference(destination), 1), r, g, b, alpha);
        charsWritten = 9;
        return true;
    }

    public static bool TryFormatHex(byte r, byte g, byte b, byte alpha, Span<char> destination, out int charsWritten)
        => alpha == byte.MaxValue
            ? TryFormatHex(r, g, b, destination, out charsWritten)
            : TryFormatHexa(r, g, b, alpha, destination, out charsWritten);
}
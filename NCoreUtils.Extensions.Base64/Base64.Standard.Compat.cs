using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NCoreUtils;

public static partial class Base64
{
    private interface IBase64Lookup
    {
        uint Lookup(char ch);
    }


    private struct StandardBase64Lookup : IBase64Lookup
    {

        private static ReadOnlySpan<uint> Base64Table =>
        [
            InvalidMark, // 0x00
            InvalidMark, // 0x01
            InvalidMark, // 0x02
            InvalidMark, // 0x03
            InvalidMark, // 0x04
            InvalidMark, // 0x05
            InvalidMark, // 0x06
            InvalidMark, // 0x07
            InvalidMark, // 0x08
            InvalidMark, // 0x09
            InvalidMark, // 0x0a
            InvalidMark, // 0x0b
            InvalidMark, // 0x0c
            InvalidMark, // 0x0d
            InvalidMark, // 0x0e
            InvalidMark, // 0x0f
            InvalidMark, // 0x10
            InvalidMark, // 0x11
            InvalidMark, // 0x12
            InvalidMark, // 0x13
            InvalidMark, // 0x14
            InvalidMark, // 0x15
            InvalidMark, // 0x16
            InvalidMark, // 0x17
            InvalidMark, // 0x18
            InvalidMark, // 0x19
            InvalidMark, // 0x1a
            InvalidMark, // 0x1b
            InvalidMark, // 0x1c
            InvalidMark, // 0x1d
            InvalidMark, // 0x1e
            InvalidMark, // 0x1f
            InvalidMark, // ' ' | 0x20
            InvalidMark, // '!' | 0x21
            InvalidMark, // '"' | 0x22
            InvalidMark, // '#' | 0x23
            InvalidMark, // '$' | 0x24
            InvalidMark, // '%' | 0x25
            InvalidMark, // '&' | 0x26
            InvalidMark, // '\'' | 0x27
            InvalidMark, // '(' | 0x28
            InvalidMark, // ')' | 0x29
            InvalidMark, // '*' | 0x2a
            62, // '+' | 0x2b
            InvalidMark, // ',' | 0x2c
            InvalidMark, // '-' | 0x2d
            InvalidMark, // '.' | 0x2e
            63, // '/' | 0x2f
            52, // '0' | 0x30
            53, // '1' | 0x31
            54, // '2' | 0x32
            55, // '3' | 0x33
            56, // '4' | 0x34
            57, // '5' | 0x35
            58, // '6' | 0x36
            59, // '7' | 0x37
            60, // '8' | 0x38
            61, // '9' | 0x39
            InvalidMark, // ':' | 0x3a
            InvalidMark, // ';' | 0x3b
            InvalidMark, // '<' | 0x3c
            InvalidMark, // '=' | 0x3d
            InvalidMark, // '>' | 0x3e
            InvalidMark, // '?' | 0x3f
            InvalidMark, // '@' | 0x40
            0, // 'A' | 0x41
            1, // 'B' | 0x42
            2, // 'C' | 0x43
            3, // 'D' | 0x44
            4, // 'E' | 0x45
            5, // 'F' | 0x46
            6, // 'G' | 0x47
            7, // 'H' | 0x48
            8, // 'I' | 0x49
            9, // 'J' | 0x4a
            10, // 'K' | 0x4b
            11, // 'L' | 0x4c
            12, // 'M' | 0x4d
            13, // 'N' | 0x4e
            14, // 'O' | 0x4f
            15, // 'P' | 0x50
            16, // 'Q' | 0x51
            17, // 'R' | 0x52
            18, // 'S' | 0x53
            19, // 'T' | 0x54
            20, // 'U' | 0x55
            21, // 'V' | 0x56
            22, // 'W' | 0x57
            23, // 'X' | 0x58
            24, // 'Y' | 0x59
            25, // 'Z' | 0x5a
            InvalidMark, // '[' | 0x5b
            InvalidMark, // '\' | 0x5c
            InvalidMark, // ']' | 0x5d
            InvalidMark, // '^' | 0x5e
            InvalidMark, // '_' | 0x5f
            InvalidMark, // '`' | 0x60
            26, // 'a' | 0x61
            27, // 'b' | 0x62
            28, // 'c' | 0x63
            29, // 'd' | 0x64
            30, // 'e' | 0x65
            31, // 'f' | 0x66
            32, // 'g' | 0x67
            33, // 'h' | 0x68
            34, // 'i' | 0x69
            35, // 'j' | 0x6a
            36, // 'k' | 0x6b
            37, // 'l' | 0x6c
            38, // 'm' | 0x6d
            39, // 'n' | 0x6e
            40, // 'o' | 0x6f
            41, // 'p' | 0x70
            42, // 'q' | 0x71
            43, // 'r' | 0x72
            44, // 's' | 0x73
            45, // 't' | 0x74
            46, // 'u' | 0x75
            47, // 'v' | 0x76
            48, // 'w' | 0x77
            49, // 'x' | 0x78
            50, // 'y' | 0x79
            51, // 'z' | 0x7a
            InvalidMark, // '{' | 0x7b
            InvalidMark, // '|' | 0x7c
            InvalidMark, // '}' | 0x7d
            InvalidMark, // '~' | 0x7e
            InvalidMark, // 0x7f
        ];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly uint Lookup(char ch)
        {
            var uch = unchecked((uint)ch);
            return uch < 0x080u
                ? Base64Table[unchecked((int)uch)]
                : InvalidMark;
        }
    }

    private ref struct ChunkBuffer(Span<char> chunk)
    {
        private Span<char> chunk = chunk;

        public int Written { get; set; } = 0;

        public int Push(char ch)
        {
            chunk[Written++] = ch;
            return Written;
        }

        public void Reset() => Written = 0;
    }

    private const uint InvalidMark = 0xFFFFFFFF;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryDecodePadded<TLookup>(TLookup lookup, ReadOnlySpan<char> base64, Span<byte> buffer, out int written)
        where TLookup : IBase64Lookup
    {
        if (base64.IsEmpty)
        {
            written = 0;
            return true;
        }
        Span<char> chunk = stackalloc char[4];
        var chunkBuffer = new ChunkBuffer(chunk);
        var emitted = 0;

        for (var i = 0; i < base64.Length; ++i)
        {
            var ch = base64[i];
            if (char.IsWhiteSpace(ch))
            {
                continue;
            }
            if (ch == '=')
            {
                // NOTE: padding, must occur only at the end!
                if (!ContainsOnlyPadding(base64[(i + 1)..]))
                {
                    written = default;
                    return false;
                }
                break;
            }
            if (chunkBuffer.Push(ch) == 4)
            {
                uint b0 = lookup.Lookup(chunk[0]);
                uint b1 = lookup.Lookup(chunk[1]);
                uint b2 = lookup.Lookup(chunk[2]);
                uint b3 = lookup.Lookup(chunk[3]);
                if (((b0 | b1 | b2 | b3) == InvalidMark) || (buffer.Length - emitted + 3 < 0))
                {
                    written = default;
                    return false;
                }
                uint combined = unchecked((b0 << 18) | (b1 << 12) | (b2 << 6) | b3);
                buffer[emitted++] = unchecked((byte)((combined >> 16) & 0x0FFu));
                buffer[emitted++] = unchecked((byte)((combined >> 8) & 0x0FFu));
                buffer[emitted++] = unchecked((byte)(combined & 0x0FFu));
                chunkBuffer.Reset();
            }
        }
        switch (chunkBuffer.Written)
        {
            case 3:
                uint b0 = lookup.Lookup(chunk[0]);
                uint b1 = lookup.Lookup(chunk[1]);
                uint b2 = lookup.Lookup(chunk[2]);
                if (((b0 | b1 | b2) == InvalidMark) || (buffer.Length - emitted + 2 < 0))
                {
                    written = default;
                    return false;
                }
                uint combined = unchecked((b0 << 10) | (b1 << 4) | (b2 >> 2));
                buffer[emitted++] = (byte)((combined >> 8) & 0x0FFu);
                buffer[emitted++] = unchecked((byte)(combined & 0x0FFu));
                break;
            case 2:
                uint bb0 = lookup.Lookup(chunk[0]);
                uint bb1 = lookup.Lookup(chunk[1]);
                if (((bb0 | bb1) == InvalidMark) || (buffer.Length - emitted + 1 < 0))
                {
                    written = default;
                    return false;
                }
                buffer[emitted++] = (byte)((bb0 << 2) | (bb1 >> 4));
                break;
            case 1:
                written = default;
                return false;
        }
        written = emitted;
        return true;

        static bool ContainsOnlyPadding(ReadOnlySpan<char> source)
        {
            foreach (var ch in source)
            {
                if (ch != '=')
                {
                    return false;
                }
            }
            return true;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool TryDecodeStandardPadded(ReadOnlySpan<char> base64, Span<byte> buffer, out int written)
        => TryDecodePadded(default(StandardBase64Lookup), base64, buffer, out written);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool TryDecodeStandardNotPadded(ReadOnlySpan<char> base64, Span<byte> buffer, out int written)
        => TryDecodePadded(default(StandardBase64Lookup), base64, buffer, out written);
}
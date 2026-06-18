using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NCoreUtils;

public static partial class Base64
{
    private struct UrlBase64Lookup : IBase64Lookup
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
            InvalidMark, // '+' | 0x2b
            InvalidMark, // ',' | 0x2c
            62, // '-' | 0x2d
            InvalidMark, // '.' | 0x2e
            InvalidMark, // '/' | 0x2f
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
            63, // '_' | 0x5f
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

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool TryDecodeUrlNotPadded(ReadOnlySpan<char> base64, Span<byte> buffer, out int written)
        => TryDecodePadded(default(UrlBase64Lookup), base64, buffer, out written);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool TryDecodeUrlPadded(ReadOnlySpan<char> base64, uint paddingSize, Span<byte> buffer, out int written)
        => TryDecodePadded(default(UrlBase64Lookup), base64, buffer, out written);
}
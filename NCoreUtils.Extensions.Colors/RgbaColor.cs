using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NCoreUtils.Colors;

public readonly record struct RgbaColor(RgbColor Rgb, byte Alpha)
    : IFormattable
#if !NETFRAMEWORK && NET6_0_OR_GREATER
    , ISpanFormattable
#if NET7_0_OR_GREATER
    , IParsable<RgbaColor>
    , ISpanParsable<RgbaColor>
#endif
#endif
{
    public static class Formats
    {
        /// <summary>
        /// "rgba(r,g,b,a)" format.
        /// </summary>
        public const string D = "D";

        /// <summary>
        /// Either "rgba(r,g,b,a)" or "rgb(r,g,b)" format depending on the alpha value. When alpha has maximum value the
        /// later format is used.
        /// </summary>
        public const string d = "d";

        /// <summary>
        /// "#RRGGBBAA" format.
        /// </summary>
        public const string X = "X";

        /// <summary>
        /// Either "#RRGGBBAA"  or "#RRGGBB" format depending on the alpha value. When alpha has maximum value the later format
        /// is used.
        /// </summary>
        public const string x = "x";
    }

    private static ReadOnlySpan<byte> CharToHexLookup =>
    [
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 15
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 31
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 47
        0x0,  0x1,  0x2,  0x3,  0x4,  0x5,  0x6,  0x7,  0x8,  0x9,  0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 63
        0xFF, 0xA,  0xB,  0xC,  0xD,  0xE,  0xF,  0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 79
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 95
        0xFF, 0xa,  0xb,  0xc,  0xd,  0xe,  0xf,  0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 111
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 127
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 143
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 159
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 175
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 191
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 207
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 223
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 239
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF  // 255
    ];

    public static readonly RgbaColor Default = new(default, byte.MaxValue);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAsciiHex(char b, out byte value)
    {
        if (b < CharToHexLookup.Length)
        {
            value = CharToHexLookup[b];
            return value != 0xFF;
        }
        value = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAsciiDigit(char b, out uint value)
    {
        value = unchecked((uint)b - '0');
        return value <= 9u;
    }

    private static bool TryParseByte(ReadOnlySpan<char> input, out byte value, out int used)
    {
        if (input.Length == 0 || !IsAsciiDigit(input[0], out var n))
        {
            (value, used) = (default, default);
            return false;
        }
        var offset = 1;
        while (offset < input.Length && IsAsciiDigit(input[offset], out var n0))
        {
            n = n * 10 + n0;
            if (n > byte.MaxValue)
            {
                (value, used) = (0, 0);
                return false;
            }
            ++offset;
        }
        (value, used) = (unchecked((byte)n), offset);
        return true;
    }

    private static ReadOnlySpan<float> Pow10 =>
    [
        1.0f,
        10.0f,
        100.0f,
        1_000.0f,
        10_000.0f,
        100_000.0f,
        1_000_000.0f,
        10_000_000.0f,
        100_000_000.0f,
        1_000_000_000.0f
    ];

    private static bool TryParseFloat(ReadOnlySpan<char> input, out float value, out int used)
    {
        if (input.Length == 0)
        {
            (value, used) = (default, default);
            return false;
        }
        uint whole;
        int offset;
        if (input[0] == '.')
        {
            whole = 0u;
            offset = 1;
            goto parseDecimal;
        }
        if (!IsAsciiDigit(input[0], out whole))
        {
            (value, used) = (default, default);
            return false;
        }
        offset = 1;
        while (offset < input.Length && IsAsciiDigit(input[offset], out var n0))
        {
            whole = whole * 10 + n0;
            if (whole > ushort.MaxValue)
            {
                (value, used) = (0, 0);
                return false;
            }
            ++offset;
        }
        if (offset == input.Length)
        {
            (value, used) = (whole, offset);
            return true;
        }
        if (input[offset] == '%')
        {
            (value, used) = (whole / 100.0f, offset + 1);
            return true;
        }
        if (input[offset] != '.')
        {
            (value, used) = (whole, offset);
            return true;
        }
        ++offset;
    parseDecimal:
        var decimals = 0;
        var frac = 0u;
        while (offset < input.Length && IsAsciiDigit(input[offset], out var n0))
        {
            frac = frac * 10 + n0;
            ++decimals;
            ++offset;
        }
        var res = whole + frac / Pow10[decimals];
        (value, used) = (res, offset);
        return true;
    }

    private static bool TryParseByteFollowedBy(ReadOnlySpan<char> input, char next, out byte value, out int used)
    {
        var offset = 0;
        while (offset < input.Length && char.IsWhiteSpace(input[offset])) { ++offset; }
        if (TryParseByte(input[offset..], out value, out var u))
        {
            offset += u;
            while (offset < input.Length && char.IsWhiteSpace(input[offset])) { ++offset; }
            if (offset >= input.Length  || input[offset] != next)
            {
                used = default;
                return false;
            }
            used = offset + 1;
            return true;
        }
        used = default;
        return false;
    }

    private static bool TryParseFloatFollowedBy(ReadOnlySpan<char> input, char next, out float value, out int used)
    {
        var offset = 0;
        while (offset < input.Length && char.IsWhiteSpace(input[offset])) { ++offset; }
        if (TryParseFloat(input[offset..], out value, out var u))
        {
            offset += u;
            while (offset < input.Length && char.IsWhiteSpace(input[offset])) { ++offset; }
            if (offset >= input.Length  || input[offset] != next)
            {
                used = default;
                return false;
            }
            used = offset + 1;
            return true;
        }
        used = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ContainsNonWhitespace(ReadOnlySpan<char> input)
    {
        foreach (var ch in input)
        {
            if (!char.IsWhiteSpace(ch))
            {
                return true;
            }
        }
        return false;
    }

    private static bool TryParseRgbSuffix(ReadOnlySpan<char> rest, out RgbaColor rgbaColor)
    {
        if (TryParseByteFollowedBy(rest, ',', out var r, out var used))
        {
            rest = rest[used..];
            if (TryParseByteFollowedBy(rest, ',', out var g, out used))
            {
                rest = rest[used..];
                if (TryParseByteFollowedBy(rest, ')', out var b, out used))
                {
                    rest = rest[used..];
                    if (rest.Length == 0 || !ContainsNonWhitespace(rest))
                    {
                        rgbaColor = unchecked(new RgbaColor(R: r, G: g, B: b, Alpha: byte.MaxValue));
                        return true;
                    }
                }
                if (TryParseByteFollowedBy(rest, ',', out b, out used))
                {
                    rest = rest[used..];
                    if (TryParseFloatFollowedBy(rest, ')', out var a, out used)
                        && a >= 0.0f && a <= 1.0f)
                    {
                        rest = rest[used..];
                        if (rest.Length == 0 || !ContainsNonWhitespace(rest))
                        {
                            var ab = unchecked((byte)Math.Round(a * 255.0f));
                            rgbaColor = unchecked(new RgbaColor(R: r, G: g, B: b, Alpha: ab));
                            return true;
                        }
                    }
                }
            }
        }
        rgbaColor = default;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> input, out RgbaColor rgbaColor)
    {
        if (input.StartsWith("rgb(", StringComparison.InvariantCulture))
        {
            return TryParseRgbSuffix(input[4..], out rgbaColor);
        }
        else if (input.StartsWith("rgba(", StringComparison.InvariantCulture))
        {
            return TryParseRgbSuffix(input[5..], out rgbaColor);
        }
        else if (input.Length == 9) // "#RRGGBBAA".Length
        {
            if (input[0] == '#'
                && IsAsciiHex(input[1], out var r0)
                && IsAsciiHex(input[2], out var r1)
                && IsAsciiHex(input[3], out var g0)
                && IsAsciiHex(input[4], out var g1)
                && IsAsciiHex(input[5], out var b0)
                && IsAsciiHex(input[6], out var b1)
                && IsAsciiHex(input[7], out var a0)
                && IsAsciiHex(input[8], out var a1))
            {
                rgbaColor = unchecked(new RgbaColor(
                    R: (byte)(((uint)r0 << 4) + r1),
                    G: (byte)(((uint)g0 << 4) + g1),
                    B: (byte)(((uint)b0 << 4) + b1),
                    Alpha: (byte)(((uint)a0 << 4) + a1)
                ));
                return true;
            }
        }
        else if (input.Length == 7) // "#RRGGBB".Length
        {
            if (input[0] == '#'
                && IsAsciiHex(input[1], out var r0)
                && IsAsciiHex(input[2], out var r1)
                && IsAsciiHex(input[3], out var g0)
                && IsAsciiHex(input[4], out var g1)
                && IsAsciiHex(input[5], out var b0)
                && IsAsciiHex(input[6], out var b1))
            {
                rgbaColor = unchecked(new RgbaColor(
                    R: (byte)(((uint)r0 << 4) + r1),
                    G: (byte)(((uint)g0 << 4) + g1),
                    B: (byte)(((uint)b0 << 4) + b1),
                    Alpha: byte.MaxValue
                ));
                return true;
            }
        }
        rgbaColor = default;
        return false;
    }

    public static bool TryParse([NotNullWhen(true)] string? input0, out RgbaColor rgbaColor)
        => TryParse(input0.AsSpan(), out rgbaColor);

    public static RgbaColor ParseOrDefault(string? input)
        => TryParse(input, out var rgbaColor) ? rgbaColor : Default;

#if NET7_0_OR_GREATER

    static bool IParsable<RgbaColor>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out RgbaColor result)
        => TryParse(s, out result);

    static RgbaColor IParsable<RgbaColor>.Parse(string s, IFormatProvider? provider)
        => TryParse(s, out var rgbaColor)
            ? rgbaColor
            : throw new FormatException();

    static bool ISpanParsable<RgbaColor>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out RgbaColor result)
        => TryParse(s, out result);

    static RgbaColor ISpanParsable<RgbaColor>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => TryParse(s, out var rgbaColor)
            ? rgbaColor
            : throw new FormatException();

#endif

    public byte R => Rgb.R;

    public byte G => Rgb.G;

    public byte B => Rgb.B;

    public RgbaColor(byte R, byte G, byte B, byte Alpha)
        : this(new(R, G, B), Alpha)
    { }

    public void Deconstruct(out byte r, out byte g, out byte b, out byte alpha)
    {
        r = R;
        g = G;
        b = B;
        alpha = Alpha;
    }

    #region TryFormat

    [MethodImpl(Opt.Inline)]
    public bool TryFormatHex(Span<char> destination, out int charsWritten)
        => RgbFormat.TryFormatHex(R, G, B, Alpha, destination, out charsWritten);

    [MethodImpl(Opt.Inline)]
    public bool TryFormatHexa(Span<char> destination, out int charsWritten)
        => RgbFormat.TryFormatHexa(R, G, B, Alpha, destination, out charsWritten);

    [MethodImpl(Opt.Inline)]
    public bool TryFormatRgb(Span<char> destination, out int charsWritten)
        => RgbFormat.TryFormatRgb(R, G, B, Alpha, destination, out charsWritten);

    [MethodImpl(Opt.Inline)]
    public bool TryFormatRgba(Span<char> destination, out int charsWritten)
        => RgbFormat.TryFormatRgba(R, G, B, Alpha, destination, out charsWritten);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format)
    {
        var fmt = format.Length == 1 ? format[0] : 'D';
        return fmt switch
        {
            'D' => TryFormatRgba(destination, out charsWritten),
            'd' => TryFormatRgb(destination, out charsWritten),
            'X' => TryFormatHexa(destination, out charsWritten),
            'x' => TryFormatHex(destination, out charsWritten),
            _ => TryFormatRgba(destination, out charsWritten)
        };
    }

#if !NETFRAMEWORK && NET6_0_OR_GREATER
    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => TryFormat(destination, out charsWritten, format);
#endif

    #endregion

    #region ToString

    [MethodImpl(Opt.Inline)]
    public string ToRgbString()
    {
        Span<char> buffer = stackalloc char[24];
        TryFormatRgb(buffer, out var size);
        return StringFactory.FromSpan(buffer[..size]);
    }

    [MethodImpl(Opt.Inline)]
    public string ToRgbaString()
    {
        Span<char> buffer = stackalloc char[24];
        TryFormatRgba(buffer, out var size);
        return StringFactory.FromSpan(buffer[..size]);
    }

    [MethodImpl(Opt.Inline)]
    public string ToHexString()
    {
        Span<char> buffer = stackalloc char[12];
        TryFormatHex(buffer, out var size);
        return StringFactory.FromSpan(buffer[..size]);
    }

    [MethodImpl(Opt.Inline)]
    public string ToHexaString()
    {
        Span<char> buffer = stackalloc char[12];
        TryFormatHexa(buffer, out var size);
        return StringFactory.FromSpan(buffer[..size]);
    }

    [MethodImpl(Opt.Inline)]
    public string ToCssString()
        => ToRgbString();

    public override string ToString()
        => ToRgbString();

    [MethodImpl(Opt.Inline)]
    public string ToString(string? format)
    {
        Span<char> buffer = stackalloc char[24];
        TryFormat(buffer, out var size, format);
        return StringFactory.FromSpan(buffer[..size]);
    }

    string IFormattable.ToString(string? format, IFormatProvider? formatProvider)
        => ToString(format);

    #endregion
}

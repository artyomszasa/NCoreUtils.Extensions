using System.Globalization;
using System.Runtime.CompilerServices;

namespace NCoreUtils.Colors;

public readonly record struct RgbColor(byte R, byte G, byte B)
    : IFormattable
#if !NETFRAMEWORK && NET6_0_OR_GREATER
    , ISpanFormattable
#endif
{
    public static RgbColor FromHsv(short h, byte s0, byte v)
    {
        // no saturation, we can return the value across the board (grayscale)
        if (s0 == 0)
        {
            return new RgbColor(v, v, v);
        }

        // which chunk of the rainbow are we in?
        var sector = h / 60d;

        // split across the decimal (ie 3.87 into 3 and 0.87)
        int i = (int)sector;
        var f = sector - i;

        var s = s0 / 100d;
        var p = v * (1 - s);
        var q = v * (1 - s * f);
        var t = v * (1 - s * (1 - f));

        byte r, g, b;

        switch(i)
        {
        case 0:
            r = v;
            g = Clamp(t);
            b = Clamp(p);
            break;

        case 1:
            r = Clamp(q);
            g = v;
            b = Clamp(p);
            break;

        case 2:
            r = Clamp(p);
            g = v;
            b = Clamp(t);
            break;

        case 3:
            r = Clamp(p);
            g = Clamp(q);
            b = v;
            break;

        case 4:
            r = Clamp(t);
            g = Clamp(p);
            b = v;
            break;

        default:
            r = v;
            g = Clamp(p);
            b = Clamp(q);
            break;
        }

        return new RgbColor(r, g, b);

        static byte Clamp(double f)
            => (byte)Math.Min(255.0, Math.Max(0.0, f));
    }

    public HsvColor ToHsv() => HsvColor.FromRgb(R, G, B);

    [MethodImpl(Opt.Inline)]
    public RgbaColor ToRgba(byte alpha)
        => new(R, G, B, alpha);

    #region TyrFormat

    [MethodImpl(Opt.Inline)]
    public bool TryFormatHex(Span<char> destination, out int charsWritten)
        => RgbFormat.TryFormatHex(R, G, B, destination, out charsWritten);

    [MethodImpl(Opt.Inline)]
    public bool TryFormatRgb(Span<char> destination, out int charsWritten)
        => RgbFormat.TryFormatRgb(R, G, B, destination, out charsWritten);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format)
    {
        var fmt = format.Length == 1 ? format[0] : 'D';
        return fmt switch
        {
            'D' or 'd' => TryFormatRgb(destination, out charsWritten),
            'X' or 'x' => TryFormatHex(destination, out charsWritten),
            _ => TryFormatRgb(destination, out charsWritten)
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
        Span<char> buffer = stackalloc char[16];
        TryFormatRgb(buffer, out var size);
        return StringFactory.FromSpan(buffer[..size]);
    }

    [MethodImpl(Opt.Inline)]
    public string ToRgbString(byte alpha)
    {
        Span<char> buffer = stackalloc char[24];
        RgbFormat.TryFormatRgb(R, G, B, alpha, buffer, out var size);
        return StringFactory.FromSpan(buffer[..size]);
    }

    [MethodImpl(Opt.Inline)]
    public string ToRgbaString(byte alpha)
    {
        Span<char> buffer = stackalloc char[24];
        RgbFormat.TryFormatRgba(R, G, B, alpha, buffer, out var size);
        return StringFactory.FromSpan(buffer[..size]);
    }

    [MethodImpl(Opt.Inline)]
    public string ToHexString()
    {
        Span<char> buffer = stackalloc char[8];
        TryFormatHex(buffer, out var size);
        return StringFactory.FromSpan(buffer[..size]);
    }

    [MethodImpl(Opt.Inline)]
    public string ToHexString(byte alpha)
    {
        Span<char> buffer = stackalloc char[12];
        RgbFormat.TryFormatHex(R, G, B, alpha, buffer, out var size);
        return StringFactory.FromSpan(buffer[..size]);
    }

    [MethodImpl(Opt.Inline)]
    public string ToHexaString(byte alpha)
    {
        Span<char> buffer = stackalloc char[12];
        RgbFormat.TryFormatHexa(R, G, B, alpha, buffer, out var size);
        return StringFactory.FromSpan(buffer[..size]);
    }

    [MethodImpl(Opt.Inline)]
    public string ToCssString()
        => ToRgbString();

    [MethodImpl(Opt.Inline)]
    public string ToCssString(byte alpha)
        => ToRgbString(alpha);

    public override string ToString()
        => ToRgbString();

    [MethodImpl(Opt.Inline)]
    public string ToString(string? format)
    {
        Span<char> buffer = stackalloc char[16];
        TryFormat(buffer, out var size, format);
        return StringFactory.FromSpan(buffer[..size]);
    }

    string IFormattable.ToString(string? format, IFormatProvider? formatProvider)
        => ToString(format);

    #endregion
}
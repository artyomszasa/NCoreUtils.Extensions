using System.Globalization;

namespace NCoreUtils.Colors;

public readonly record struct HsvColor(short Hue, byte Saturation, byte Value)
{
    public static HsvColor FromRgb(byte r, byte g, byte b)
    {
        var max = Math.Max(r, Math.Max((int)g, b));
        var min = Math.Min(r, Math.Min((int)g, b));
        var delta = max - min;

        var hue = delta == 0
            ? 0d
            : r == max
                ? (double)(g - b) / delta
                : g == max
                    ? 2 + (double)(b - r) / delta
                    : 4 + (double)(r - g) / delta;
        var saturation = (max == 0) ? 0 : 1d - ((double)min / max);
        return new HsvColor(
            Hue: (short)Math.Max(0d, Math.Min(360d, hue * 60d)),
            Saturation: (byte)Math.Max(0, Math.Min(100, Math.Round(saturation * 100d))),
            Value: unchecked((byte)max)
        );
    }

    public RgbColor ToRgb() => RgbColor.FromHsv(Hue, Saturation, Value);

    public override string ToString()
#if NETFRAMEWORK || NETSTANDARD2_0 || NETSTANDARD2_1
        => $"hlv({Hue},{Saturation},{Value})";
#else
        => string.Create(CultureInfo.InvariantCulture, $"hlv({Hue},{Saturation},{Value})");
#endif
}
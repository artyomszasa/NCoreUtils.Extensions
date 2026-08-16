namespace NCoreUtils.Colors;

public readonly record struct HsvaColor(HsvColor Hsv, byte Alpha)
{
    public static readonly HsvaColor Default = new(default, byte.MaxValue);

    public short Hue => Hsv.Hue;

    public byte Saturation => Hsv.Saturation;

    public byte Value => Hsv.Value;

    public HsvaColor(short hue, byte saturation, byte value, byte alpha)
        : this(new(hue, saturation, value), alpha)
    { }

    public override string ToString()
        => $"hlva({Hue},{Saturation},{Value},{Alpha})";
}
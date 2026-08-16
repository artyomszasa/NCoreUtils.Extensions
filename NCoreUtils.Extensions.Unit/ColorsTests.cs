using System;
using NCoreUtils.Colors;
using Xunit;

namespace NCoreUtils.Extensions.Unit;

public class ColorsTests
{
    [Theory]
    [InlineData("#00000000", "#00000000", "rgb(0,0,0,0)", "rgb(0,0,0,0)", 0, 0, 0, 0)]
    [InlineData("#000000FF", "#000000", "rgb(0,0,0)", "rgb(0,0,0,1)", 0, 0, 0, 255)]
    [InlineData("#101010FF", "#101010", "rgb(16,16,16)", "rgb(16,16,16,1)", 16, 16, 16, 255)]
    [InlineData("#7F7F7F80", "#7F7F7F80", "rgb(127,127,127,0.5)", "rgb(127,127,127,0.5)", 127, 127, 127, 128)]
    public void Rgba(string hexa, string hex, string rgb, string rgba, byte r, byte g, byte b, byte a)
    {
        var v = new RgbaColor(r, g, b, a);
        Assert.Equal(hexa, v.ToHexaString());
        Assert.Equal(hex, v.ToHexString());
        Assert.Equal(rgba, v.ToRgbaString());
        Assert.Equal(rgb, v.ToRgbString());

        Assert.Equal(hexa, v.ToString("X"));
        Assert.Equal(hex, v.ToString("x"));
        Assert.Equal(rgba, v.ToString("D"));
        Assert.Equal(rgb, v.ToString("d"));
        Assert.Equal(rgba, v.ToString("u"));
        Assert.Equal(rgb, v.ToString());
        Assert.Equal(rgba, ((IFormattable)v).ToString("D", default));

        var (r1, g1, b1, a1) = v;
        Assert.Equal(r, r1);
        Assert.Equal(g, g1);
        Assert.Equal(b, b1);
        Assert.Equal(a, a1);

        Assert.True(RgbaColor.TryParse(hexa, out var vx));
        Assert.Equal(v, vx);
        Assert.True(RgbaColor.TryParse(hex, out vx));
        Assert.Equal(v, vx);
        Assert.True(RgbaColor.TryParse(rgba, out vx));
        Assert.Equal(v, vx);
        Assert.True(RgbaColor.TryParse(rgb, out vx));
        Assert.Equal(v, vx);
    }

    [Theory]
    [InlineData("rgb(1,1,1,50%)", 1, 1, 1, 128)]
    [InlineData("rgb(1,1,1,0.5)", 1, 1, 1, 128)]
    [InlineData("rgb(1,1,1,.5)", 1, 1, 1, 128)]
    [InlineData("rgb(1,1,1,.5)   ", 1, 1, 1, 128)]
    [InlineData("rgba(1,1,1,.5)   ", 1, 1, 1, 128)]
    [InlineData("#01010180", 1, 1, 1, 128)]
    public void RgbaParse(string raw, byte r, byte g, byte b, byte a)
    {
        Assert.True(RgbaColor.TryParse(raw, out var v));
        Assert.Equal(new RgbaColor(r, g, b, a), v);
    }

    [Theory]
    [InlineData("rgb(256,0,0,)")]
    [InlineData("rgb(0,0,0,)")]
    [InlineData("rgb(0,0,0,")]
    [InlineData("rgb(0,0,0,100.2)")]
    [InlineData("rgb(0,0,0,100000000)")]
    [InlineData("rgb(0,0,0,1")]
    [InlineData("rgb(0,0,0,1.0")]
    [InlineData("rgb(0,0,0,50%")]
    [InlineData("rgb(0,0,0,50%)x")]
    [InlineData("rgb(x,0,0,50%)")]
    [InlineData("#0G0000")]
    [InlineData("#0É0000")]
    public void RgbaInvalid(string raw)
    {
        Assert.False(RgbaColor.TryParse(raw, out _));
    }

}
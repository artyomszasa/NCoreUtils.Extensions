using System;
using System.Globalization;
using Xunit;

namespace NCoreUtils.Extensions;

public class YearAndMonthTests
{
    private static void CheckSpanFormatting(YearAndMonth ym, string expected)
    {
        var bufferFail = new char[expected.Length - 1];
        var bufferSucc = new char[expected.Length];
        Assert.False(ym.TryFormat(bufferFail, out _, default, default));
        Assert.True(ym.TryFormat(bufferSucc, out var written, default, default));
        Assert.Equal(expected.Length, written);
        Assert.Equal(expected, new string(bufferSucc, 0, expected.Length));
    }

    [Fact]
    public void BasicTests()
    {
        var ym = new YearAndMonth(2024, 6);
        Assert.Equal("1-06", new YearAndMonth(1, 6).ToString());
        Assert.Equal("10-06", new YearAndMonth(10, 6).ToString());
        Assert.Equal("100-06", new YearAndMonth(100, 6).ToString());
        Assert.Equal("2024-06", ym.ToString());
        Assert.Equal("2024-06", ((IFormattable)ym).ToString("x", CultureInfo.InvariantCulture));
        Assert.True(ym == new YearAndMonth(2024, 6));
        Assert.True(((object)ym).Equals(new YearAndMonth(2024, 6)));
        Assert.False(ym.Equals("yy"));
        Assert.False(ym != new YearAndMonth(2024, 6));
        Assert.True(ym <= new YearAndMonth(2024, 6));
        Assert.True(ym >= new YearAndMonth(2024, 6));
        Assert.True(ym < new YearAndMonth(2024, 7));
        Assert.True(ym > new YearAndMonth(2024, 5));
        Assert.False(ym < new YearAndMonth(2024, 5));
        Assert.False(ym > new YearAndMonth(2024, 7));
        Assert.Equal("2024-07", ym.AddMonths(1).ToString());
        Assert.Equal("2023-06", ym.AddMonths(-12).ToString());
        Assert.Equal("2025-06", ym.AddYears(1).ToString());
        Assert.Equal("2023-06", ym.AddYears(-1).ToString());
        Assert.Equal(0, ym.CompareTo(new YearAndMonth(2024, 6)));
        Assert.True(ym.CompareTo(new YearAndMonth(2023, 6)) > 0);
        Assert.True(ym.CompareTo(new YearAndMonth(2025, 6)) < 0);
        Assert.True(((IComparable)ym).CompareTo((object)new YearAndMonth(2025, 6)) < 0);
        // SpanFormat
        Assert.False(ym.TryFormat(Span<char>.Empty, out _, default, default));
        CheckSpanFormatting(ym, "2024-06");
        CheckSpanFormatting(new YearAndMonth(1, 6), "1-06");
        CheckSpanFormatting(new YearAndMonth(10, 6), "10-06");
        CheckSpanFormatting(new YearAndMonth(100, 6), "100-06");
        CheckSpanFormatting(new YearAndMonth(10000, 6), "10000-06");
    }

    [Fact]
    public void InvalidCaseHandling()
    {
        Assert.Equal(
            "year",
            Assert.Throws<ArgumentOutOfRangeException>(() => new YearAndMonth(0, 6)).ParamName
        );
        Assert.Equal(
            "month",
            Assert.Throws<ArgumentOutOfRangeException>(() => new YearAndMonth(2000, 0)).ParamName
        );
        var ym = new YearAndMonth(2024, 6);
        var (y, m) = ym;
        Assert.Equal(2024, y);
        Assert.Equal(6, m);
        Assert.Equal(ym.GetHashCode(), new YearAndMonth(2024, 6).GetHashCode());
        Assert.Throws<InvalidOperationException>(() => ym.AddYears(-3000));
        Assert.Throws<InvalidOperationException>(() => ym.AddYears(ushort.MaxValue + 1));
        Assert.Throws<InvalidOperationException>(() => ym.AddMonths(-3000 * 12));
        Assert.Throws<InvalidOperationException>(() => YearAndMonth.MaxValue.AddMonths(1));
        Assert.Throws<InvalidOperationException>(() => YearAndMonth.MaxValue.CompareTo("12"));
        Assert.Throws<InvalidOperationException>(() => YearAndMonth.MaxValue.CompareTo(null));
        Assert.Throws<InvalidOperationException>(() => YearAndMonth.MinValue.AddMonths(-1));

    }
}
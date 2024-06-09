using System;
using System.Globalization;
using Xunit;

namespace NCoreUtils.Extensions
{
    public class RawDateTimeTests
    {
        [Fact]
        public void BasicTests()
        {
            var dt = new RawDateTime(2020, 10, 5, 12, 13, 14, 400);
            Assert.Equal(2020, dt.Year);
            Assert.Equal(10, dt.Month);
            Assert.Equal(5, dt.Day);
            Assert.Equal(12, dt.Hour);
            Assert.Equal(13, dt.Minute);
            Assert.Equal(14, dt.Second);
            Assert.Equal(400, dt.Millisecond);
            Assert.Equal(0, RawDateTime.Zero.Year);
            Assert.Equal(0, RawDateTime.Zero.Month);
            // EQ
            Assert.Equal(new RawDateTime(2020, 10, 5), new RawDateTime(new DateOnly(2020, 10, 5)));
            Assert.Equal(new RawDateTime(2020, 10, 5), new RawDateTime(new DateOnly(2020, 10, 5), TimeOnly.MinValue));
            Assert.Equal(new RawDateTime(2020, 10, 5), new RawDateTime(2020, 10, 5, TimeOnly.MinValue));
            Assert.Equal(new RawDateTime(2020, 10, 5).GetHashCode(), new RawDateTime(2020, 10, 5, TimeOnly.MinValue).GetHashCode());
            Assert.True(dt.Equals(new RawDateTime(new DateTime(2020, 10, 5, 12, 13, 14, 400))));
            Assert.True(dt.Equals(new RawDateTime(new DateTimeOffset(2020, 10, 5, 12, 13, 14, 400, TimeSpan.Zero))));
            Assert.True(dt.Equals(new RawDateTime(2020, 10, 5, 12, 13, 14, 400)));
            Assert.False(dt.Equals(new RawDateTime(2020, 10, 6, 12, 13, 14, 400)));
            Assert.True(dt == new RawDateTime(2020, 10, 5, 12, 13, 14, 400));
            Assert.False(dt != new RawDateTime(2020, 10, 5, 12, 13, 14, 400));
            Assert.True(dt > new RawDateTime(2020, 10, 5, 12, 13, 13, 400));
            Assert.True(dt < new RawDateTime(2020, 10, 5, 12, 13, 15, 400));
            Assert.True(dt >= new RawDateTime(2020, 10, 5, 12, 13, 13, 400));
            Assert.True(dt <= new RawDateTime(2020, 10, 5, 12, 13, 15, 400));
            Assert.True(dt >= new RawDateTime(2020, 10, 5, 12, 13, 14, 400));
            Assert.True(dt <= new RawDateTime(2020, 10, 5, 12, 13, 14, 400));
            Assert.True(dt.Equals((object)new RawDateTime(2020, 10, 5, 12, 13, 14, 400)));
            Assert.Equal(0, dt.CompareTo((object)new RawDateTime(2020, 10, 5, 12, 13, 14, 400)));
            Assert.False(dt.Equals("xxx"));
            Assert.False(dt.Equals(null));
            // ..
            Assert.Equal(new DateOnly(2020, 10, 5), new RawDateTime(2020, 10, 5, 12, 13, 14, 400).Date);
            Assert.Equal(default, default(RawDateTime).ToDateTimeOffset(TimeSpan.FromHours(1)));
            Assert.Equal(new DateTimeOffset(2020, 10, 5, 0, 0, 0, TimeSpan.Zero), new RawDateTime(2020, 10, 5).ToDateTimeOffset(TimeSpan.Zero));
        }

        [Fact]
        public void StringifyTests()
        {
            var chu = new CultureInfo("hu-HU");
            var rdt = new RawDateTime(2020, 10, 25, 1);
            var dt = new DateTime(2020, 10, 25, 1, 0, 0, DateTimeKind.Unspecified);
            Assert.Equal(dt.ToString("G", CultureInfo.CurrentCulture), rdt.ToString());
            Assert.Equal(dt.ToString("G", CultureInfo.CurrentCulture), rdt.ToString("G"));
            Assert.Equal(dt.ToString("G", chu), rdt.ToString(chu));
            Assert.Equal(dt.ToString("G", chu), ((IConvertible)rdt).ToType(typeof(string), chu));
            Assert.Equal(dt.ToString("o", chu), rdt.ToString("o", chu));
            Assert.Equal(string.Empty, default(RawDateTime).ToString("o", chu));

            char[] buffer0 = new char[64];
            char[] buffer1 = new char[64];
            Assert.True(dt.TryFormat(buffer0, out var written0));
            Assert.True(rdt.TryFormat(buffer1, out var written1));
            Assert.Equal(written0, written1);
            Assert.Equal(new string(buffer0, 0, written0), new string(buffer1, 0, written1));

            Assert.Equal(DateTime.SpecifyKind(dt, DateTimeKind.Local), rdt.ToLocalDateTime());

            Assert.Equal(default, default(RawDateTime).ToLocalDateTime());

            Assert.Equal(dt, ((IConvertible)rdt).ToDateTime(default));
            Assert.Equal(dt, ((IConvertible)rdt).ToType(typeof(DateTime), default));

        }

        [Fact]
        public void ParseTests()
        {
            var dt = new DateTime(2020, 10, 25, 1, 0, 0, DateTimeKind.Unspecified);
            Assert.Equal(new RawDateTime(dt), RawDateTime.Parse(dt.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
            Assert.Equal(new RawDateTime(dt), RawDateTime.Parse(dt.ToString(CultureInfo.InvariantCulture).AsSpan(), CultureInfo.InvariantCulture));
            Assert.False(RawDateTime.TryParse("sksjhksjdfa", default, out _));
            Assert.False(RawDateTime.TryParse("sksjhksjdfa".AsSpan(), default, out _));
        }

        [Fact]
        public void DailightSavingTests()
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Budapest");
            var rdt0 = new RawDateTime(2020, 10, 25, 1);
            var rdt1 = new RawDateTime(2020, 10, 25, 3);
            var dt0 = rdt0.ToDateTimeOffset(tz);
            var dt1 = rdt1.ToDateTimeOffset(tz);
            Assert.Equal(new DateTimeOffset(2020, 10, 25, 1, 0, 0, TimeSpan.FromHours(2)), dt0);
            Assert.Equal(new DateTimeOffset(2020, 10, 25, 3, 0, 0, TimeSpan.FromHours(1)), dt1);
            Assert.Equal(new DateTimeOffset(2020, 10, 25, 1, 0, 0, TimeSpan.FromHours(2)), TimeZoneInfoExtensions.DateTimeOffsetFor(tz, 2020, 10, 25, TimeSpan.FromHours(1)));
            Assert.Equal(new DateTimeOffset(2020, 10, 25, 3, 0, 0, TimeSpan.FromHours(1)), TimeZoneInfoExtensions.DateTimeOffsetFor(tz, 2020, 10, 25, TimeSpan.FromHours(3)));
            Assert.Equal(new DateTimeOffset(2020, 10, 25, 1, 0, 0, TimeSpan.FromHours(2)), TimeZoneInfoExtensions.DateTimeOffsetFor(tz, new DateOnly(2020, 10, 25), TimeSpan.FromHours(1)));
            Assert.Equal(new DateTimeOffset(2020, 10, 25, 3, 0, 0, TimeSpan.FromHours(1)), TimeZoneInfoExtensions.DateTimeOffsetFor(tz, new DateOnly(2020, 10, 25), TimeSpan.FromHours(3)));
            Assert.Equal(new DateTimeOffset(2020, 10, 25, 1, 0, 0, TimeSpan.FromHours(2)), TimeZoneInfoExtensions.DateTimeOffsetFor(tz, new DateOnly(2020, 10, 25), new TimeOnly(1, 0)));
            Assert.Equal(new DateTimeOffset(2020, 10, 25, 3, 0, 0, TimeSpan.FromHours(1)), TimeZoneInfoExtensions.DateTimeOffsetFor(tz, new DateOnly(2020, 10, 25), new TimeOnly(3, 0)));
            Assert.Equal(default, default(RawDateTime).ToDateTimeOffset(tz));
        }

        [Fact]
        public void InvalidCaseHandling()
        {
            var c = (IConvertible)new RawDateTime(2020, 10, 25);
            Assert.Equal(TypeCode.Object, c.GetTypeCode());
            Assert.Equal(
                "year",
                Assert.Throws<ArgumentOutOfRangeException>(() => new RawDateTime(-1, 10, 25)).ParamName
            );
            Assert.Equal(
                "year",
                Assert.Throws<ArgumentOutOfRangeException>(() => new RawDateTime(5000, 10, 25)).ParamName
            );
            Assert.Equal(
                "month",
                Assert.Throws<ArgumentOutOfRangeException>(() => new RawDateTime(2020, 0, 25)).ParamName
            );
            Assert.Equal(
                "month",
                Assert.Throws<ArgumentOutOfRangeException>(() => new RawDateTime(2020, 15, 25)).ParamName
            );
            Assert.Equal(
                "day",
                Assert.Throws<ArgumentOutOfRangeException>(() => new RawDateTime(2020, 10, 0)).ParamName
            );
            Assert.Equal(
                "day",
                Assert.Throws<ArgumentOutOfRangeException>(() => new RawDateTime(2020, 10, 400)).ParamName
            );
            Assert.Equal(
                "time",
                Assert.Throws<ArgumentOutOfRangeException>(() => new RawDateTime(2020, 10, 25, TimeSpan.FromMinutes(-1))).ParamName
            );
            Assert.Equal(
                "time",
                Assert.Throws<ArgumentOutOfRangeException>(() => new RawDateTime(2020, 10, 25, TimeSpan.FromDays(2))).ParamName
            );
            Assert.Throws<InvalidOperationException>(() => new RawDateTime(2020, 10, 25).CompareTo("xxx"));
            Assert.Throws<InvalidOperationException>(() => new RawDateTime(2020, 10, 25).CompareTo(null));
            Assert.Throws<InvalidCastException>(() => c.ToBoolean(default));
            Assert.Throws<InvalidCastException>(() => c.ToChar(default));
            Assert.Throws<InvalidCastException>(() => c.ToSByte(default));
            Assert.Throws<InvalidCastException>(() => c.ToInt16(default));
            Assert.Throws<InvalidCastException>(() => c.ToInt32(default));
            Assert.Throws<InvalidCastException>(() => c.ToInt64(default));
            Assert.Throws<InvalidCastException>(() => c.ToByte(default));
            Assert.Throws<InvalidCastException>(() => c.ToUInt16(default));
            Assert.Throws<InvalidCastException>(() => c.ToUInt32(default));
            Assert.Throws<InvalidCastException>(() => c.ToUInt64(default));
            Assert.Throws<InvalidCastException>(() => c.ToSingle(default));
            Assert.Throws<InvalidCastException>(() => c.ToDouble(default));
            Assert.Throws<InvalidCastException>(() => c.ToDecimal(default));
            Assert.Throws<InvalidCastException>(() => c.ToType(typeof(ValueTuple<int, int>), default));
            Assert.Throws<FormatException>(() => RawDateTime.Parse("sksjhksjdfa", default));
            Assert.Throws<FormatException>(() => RawDateTime.Parse("sksjhksjdfa".AsSpan(), default));
        }
    }
}
using System;
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
        }
    }
}
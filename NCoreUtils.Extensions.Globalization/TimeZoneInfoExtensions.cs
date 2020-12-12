using System;
using System.Runtime.CompilerServices;

namespace NCoreUtils
{
    public static class TimeZoneInfoExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTimeOffset DateTimeOffsetFor(this TimeZoneInfo timeZone, int year, int month, int day, TimeSpan time = default)
            => new RawDateTime(year, month, day, time).ToDateTimeOffset(timeZone);
    }
}
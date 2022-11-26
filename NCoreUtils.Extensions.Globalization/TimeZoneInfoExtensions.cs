using System;
using System.Runtime.CompilerServices;

namespace NCoreUtils
{
    public static class TimeZoneInfoExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTimeOffset DateTimeOffsetFor(this TimeZoneInfo timeZone, int year, int month, int day, TimeSpan time = default)
            => new RawDateTime(year, month, day, time).ToDateTimeOffset(timeZone);

#if NET6_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTimeOffset DateTimeOffsetFor(this TimeZoneInfo timeZone, DateOnly date, TimeSpan time = default)
            => timeZone.DateTimeOffsetFor(date.Year, date.Month, date.Day, time);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTimeOffset DateTimeOffsetFor(this TimeZoneInfo timeZone, DateOnly date, TimeOnly time)
            => timeZone.DateTimeOffsetFor(date, time.ToTimeSpan());
#endif
    }
}
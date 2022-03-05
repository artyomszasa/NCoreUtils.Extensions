using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace NCoreUtils
{
    public partial struct RawDateTime : IConvertible, IComparable, IEquatable<RawDateTime>, IComparable<RawDateTime>, IFormattable
    {
        private static readonly TimeSpan _oneDay = TimeSpan.FromDays(1);

        public static RawDateTime Zero => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TimeSpan CreateTimeSpan(int hour, int minute, int second, int millisecond, long tick)
        {
            var totalTicks = tick
                + millisecond * TimeSpan.TicksPerMillisecond
                + second * TimeSpan.TicksPerSecond
                + minute * TimeSpan.TicksPerMinute
                + hour * TimeSpan.TicksPerHour;
            return new TimeSpan(totalTicks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool MatchAdjustmentRule(in RawDateTime value, TimeZoneInfo.AdjustmentRule rule)
        {
            var ds = rule.DateStart;
            var de = rule.DateEnd;
            if (ds.Year > 4095 || de.Year > 4095)
            {
                return false;
            }
            var ts = rule.DaylightTransitionStart.TimeOfDay;
            var te = rule.DaylightTransitionEnd.TimeOfDay;
            var s = new RawDateTime(ds.Year, ds.Month, ds.Day, ts.Hour, ts.Minute, ts.Second, ts.Millisecond, ts.Ticks % TimeSpan.TicksPerMillisecond);
            var e = new RawDateTime(de.Year, de.Month, de.Day, te.Hour, te.Minute, te.Second, te.Millisecond, te.Ticks % TimeSpan.TicksPerMillisecond);
            return value.CompareTo(s) >= 0 && value.CompareTo(e) <= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator==(RawDateTime a, RawDateTime b)
            => a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator!=(RawDateTime a, RawDateTime b)
            => !a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator<(RawDateTime a, RawDateTime b)
            => a.CompareTo(b) < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator>(RawDateTime a, RawDateTime b)
            => a.CompareTo(b) > 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator<=(RawDateTime a, RawDateTime b)
            => a.CompareTo(b) <= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator>=(RawDateTime a, RawDateTime b)
            => a.CompareTo(b) >= 0;


        // year  --> [60..49] // 12 bits
        // month --> [48..45] // 4 bits
        // day   --> [44..40] // 5 bits
        // time  --> [39..00] // 40 bits

        private readonly long _value;

        public TimeSpan Time
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(_value & 0x00_00_00_FF_FF_FF_FF_FFL);
        }

        public long Ticks
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Time.Ticks % TimeSpan.TicksPerMillisecond;
        }

        public int Millisecond
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Time.Milliseconds;
        }

        public int Second
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Time.Seconds;
        }

        public int Minute
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Time.Minutes;
        }

        public int Hour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Time.Hours;
        }

        public int Day => unchecked ((int)((_value & 0x00_00_1F_00_00_00_00_00L) >> 40));

        public int Month => unchecked ((int)((_value & 0x00_01_E0_00_00_00_00_00L) >> 45));

        public int Year => unchecked ((int)((_value & 0x1F_FE_00_00_00_00_00_00L) >> 49));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawDateTime(
            int year,
            int month,
            int day,
            int hour,
            int minute = default,
            int second = default,
            int millisecond = default,
            long tick = default)
            : this(year, month, day, CreateTimeSpan(hour, minute, second, millisecond, tick))
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawDateTime(
            int year,
            int month,
            int day,
            TimeSpan time = default)
        {
            if (year < 0 || year > 4095)
            {
                throw new ArgumentOutOfRangeException(nameof(year));
            }
            if (month <= 0 || month > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(month));
            }
            if (day < 0 || day > 4095)
            {
                throw new ArgumentOutOfRangeException(nameof(year));
            }
            if (time >= _oneDay || time.Ticks < 0L)
            {
                throw new ArgumentOutOfRangeException(nameof(time), "Time must be at least Zero and less than one day.");
            }
            _value = time.Ticks | ((long)day << 40) | ((long)month << 45) | ((long)year << 49);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawDateTime(DateTime source)
            : this(source.Year, source.Month, source.Day, source.TimeOfDay)
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawDateTime(DateTimeOffset source)
            : this(source.Year, source.Month, source.Day, source.TimeOfDay)
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(RawDateTime other)
            => _value == other._value;

        public override bool Equals(object? obj)
            => obj is RawDateTime other && Equals(other);

        public override int GetHashCode()
            => _value.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(RawDateTime other)
            => _value == other._value ? 0 : _value < other._value ? -1 : 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(object? obj)
            => obj switch
            {
                null => throw new InvalidOperationException("Unable to compare <null> to RawDateTime."),
                RawDateTime other => CompareTo(other),
                _ => throw new InvalidOperationException($"Unable to compare {obj.GetType()} to RawDateTime.")
            };

        /// <summary>
        /// Returns <see cref="DateTimeOffset" /> instance with the specified offset.
        /// <para>
        /// NOTE: default <see cref="DateTimeOffset" /> value is returned if the actual instance is <see cref="Zero" />.
        /// </para>
        /// </summary>
        /// <param name="offset">Offset to use.</param>
        public DateTimeOffset ToDateTimeOffset(TimeSpan offset)
        {
            if (0L == _value)
            {
                return default;
            }
            return new(Year, Month, Day, Hour, Minute, Second, Millisecond, offset);
        }

        /// <summary>
        /// Returns <see cref="DateTimeOffset" /> instance for the specified time zone with respect to the adjustments
        /// rules.
        /// <para>
        /// NOTE: default <see cref="DateTimeOffset" /> value is returned if the actual instance is <see cref="Zero" />.
        /// </para>
        /// </summary>
        /// <param name="tz">Requested time zone.</param>
        public DateTimeOffset ToDateTimeOffset(TimeZoneInfo tz)
        {
            if (0L == _value)
            {
                return default;
            }
            var offset = tz.BaseUtcOffset;
            // apply rules if any
            var rules = tz.GetAdjustmentRules();
            foreach (var rule in rules)
            {
                // always prefer adjusted time
                if (rule.DaylightDelta != TimeSpan.Zero && MatchAdjustmentRule(in this, rule))
                {
                    offset += rule.DaylightDelta;
                }
            }
            // FIXME: optimize
            var dtNoTicks = new DateTimeOffset(Year, Month, Day, Hour, Minute, Second, Millisecond, offset);
            return new DateTimeOffset(dtNoTicks.Ticks + Ticks, dtNoTicks.Offset);
        }

        /// <summary>
        /// Returns <see cref="DateTime" /> instance with <see cref="DateTime.Kind" /> property set to
        /// <see cref="DateTimeKind.Local" />.
        /// <para>
        /// NOTE: default <see cref="DateTime" /> value is returned if the actual instance is <see cref="Zero" />.
        /// </para>
        /// </summary>
        public DateTime ToLocalDateTime()
        {
            if (0L == _value)
            {
                return default;
            }
            return new(Year, Month, Day, Hour, Minute, Second, Millisecond, DateTimeKind.Local);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (0L == _value)
            {
                return string.Empty;
            }
            return new DateTime(Year, Month, Day, Hour, Minute, Second, Millisecond, DateTimeKind.Unspecified).ToString(format, formatProvider);
        }

        public string ToString(string format)
            => ToString(format, CultureInfo.CurrentCulture);

        public string ToString(IFormatProvider? formatProvider)
            => ToString("G", formatProvider);

        public override string ToString()
            => ToString("G", CultureInfo.CurrentCulture);
    }
}
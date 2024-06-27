using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class Rfc3339DateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    private static readonly string[] _formats =
    {
        "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK",
        "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffffK",
        "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffK",
        "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffK",
        "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK",
        "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffK",
        "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fK",
        "yyyy'-'MM'-'dd'T'HH':'mm':'ssK",
        // Fall back patterns
        DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern,
        DateTimeFormatInfo.InvariantInfo.SortableDateTimePattern
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAsciiDigit(byte b, out uint value)
    {
        value = unchecked((uint)b - (byte)'0');
        return value <= 9u;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryParseNonFractional(ReadOnlySpan<byte> input, out (int Year, int Month, int Day, int Hour, int Minute, int Second) value)
    {
        if (input.Length >= 19
            && IsAsciiDigit(input[0], out var y0)
            && IsAsciiDigit(input[1], out var y1)
            && IsAsciiDigit(input[2], out var y2)
            && IsAsciiDigit(input[3], out var y3)
            && unchecked((byte)'-') == input[4]
            && IsAsciiDigit(input[5], out var m0)
            && IsAsciiDigit(input[6], out var m1)
            && unchecked((byte)'-') == input[7]
            && IsAsciiDigit(input[8], out var d0)
            && IsAsciiDigit(input[9], out var d1)
            && unchecked((byte)'T') == input[10]
            && IsAsciiDigit(input[11], out var h0)
            && IsAsciiDigit(input[12], out var h1)
            && unchecked((byte)':') == input[13]
            && IsAsciiDigit(input[14], out var min0)
            && IsAsciiDigit(input[15], out var min1)
            && unchecked((byte)':') == input[16]
            && IsAsciiDigit(input[17], out var s0)
            && IsAsciiDigit(input[18], out var s1))
        {
            var year = unchecked((int)(y0 * 1000u + y1 * 100u + y2 * 10 + y3));
            var month = unchecked((int)(m0 * 10u + m1));
            var day = unchecked((int)(d0 * 10u + d1));
            var hour = unchecked((int)(h0 * 10u + h1));
            var minute = unchecked((int)(min0 * 10u + min1));
            var second = unchecked((int)(s0 * 10u + s1));
            value = (year, month, day, hour, minute, second);
            return true;
        }
        value = default;
        return false;
    }

    private static ReadOnlySpan<double> PowersOf10 =>
    [
        1.0,
        10.0,
        100.0,
        1000.0,
        10000.0,
        100000.0,
        1000000.0,
        10000000.0,
        100000000.0,
        1000000000.0,
        10000000000.0,
        100000000000.0,
        1000000000000.0,
        10000000000000.0,
    ];

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.NoInlining)]
#endif
    private static bool TryParseFastPath(ReadOnlySpan<byte> input, out DateTimeOffset value)
    {
        if (TryParseNonFractional(input, out var dateTime))
        {
            var rest = input[19 .. ];
            if (rest.Length == 0)
            {
                value = default;
                return false;
            }
            double secFraction = .0;
            if (rest[0] == (byte)'.')
            {
                rest = rest[1 .. ];
                var num = 0;
                var frac = 0u;
                while (true)
                {
                    if (rest.Length <= num)
                    {
                        value = default;
                        return false;
                    }
                    if (IsAsciiDigit(rest[num], out var f))
                    {
                        frac = frac * 10u + f;
                        ++num;
                    }
                    else
                    {
                        break;
                    }
                }
                secFraction = frac / PowersOf10[num];
                rest = rest[num .. ];
            }
            if (rest[0] == (byte)'Z')
            {
                if (rest.Length != 1)
                {
                    value = default;
                    return false;
                }
                var (year, month, day, hour, minute, second) = dateTime;
                value = new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.Zero) + TimeSpan.FromSeconds(secFraction);
                return true;
            }
            // FIXME: parse timezone
        }
        value = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryParseFastPath(ReadOnlySequence<byte> input, out DateTimeOffset value)
    {
        if (input.Length > 128)
        {
            value = default;
            return false;
        }
        Span<byte> buffer = stackalloc byte[unchecked((int)input.Length)];
        input.CopyTo(buffer);
        return TryParseFastPath(buffer, out value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryParseFastPath(in Utf8JsonReader reader, out DateTimeOffset value)
    {
        if (reader.HasValueSequence)
        {
            return TryParseFastPath(reader.ValueSequence, out value);
        }
        return TryParseFastPath(reader.ValueSpan, out value);
    }

    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.Null => default,
            JsonTokenType.String => TryParseFastPath(in reader, out var result)
                ? (DateTimeOffset?)result
                : DateTimeOffset.ParseExact(reader.GetString() ?? string.Empty, _formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces),
            var tokenType => throw new JsonException($"Unable to convert sequence starting with {tokenType} to DateTimeOffset.")
        };

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture));
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
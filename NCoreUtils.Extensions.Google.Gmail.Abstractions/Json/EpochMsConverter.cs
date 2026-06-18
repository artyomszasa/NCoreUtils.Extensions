using System.Buffers;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail.Json;

public sealed class EpochMsConverter : JsonConverter<DateTimeOffset>
{
    private static bool TryParseInt64(in Utf8JsonReader reader, out long value)
    {
        if (reader.HasValueSequence)
        {
            var seq = reader.ValueSequence;
            if (seq.Length > 19 /* "9223372036854775807".Length */)
            {
                value = default;
                return false;
            }
            Span<byte> buffer = stackalloc byte[unchecked((int)seq.Length)];
            seq.CopyTo(buffer);
            return long.TryParse(buffer, NumberStyles.None, CultureInfo.InvariantCulture, out value);
        }
        return long.TryParse(reader.ValueSpan, NumberStyles.None, CultureInfo.InvariantCulture, out value);
    }

    private static bool TryParseDateTimeOffset(in Utf8JsonReader reader, out DateTimeOffset value)
    {
        if (TryParseInt64(in reader, out var epochMs))
        {
            value = DateTimeOffset.FromUnixTimeMilliseconds(epochMs);
            return true;
        }
        return DateTimeOffset.TryParse(reader.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal, out value);
    }

    public static DateTimeOffset Read(ref Utf8JsonReader reader)
        => reader.TokenType switch
        {
            JsonTokenType.String => TryParseDateTimeOffset(in reader, out var result)
                ? result
                : throw new JsonException($"Could not parse \"{reader.GetString()}\" as DateTimeOffset (epoch ms)."),
            JsonTokenType.Number => reader.TryGetInt64(out var epochMs)
                ? DateTimeOffset.FromUnixTimeMilliseconds(epochMs)
                : throw new JsonException($"oould not parse \"{reader.GetString()}\" as DateTimeOffset (epoch ms)."),
            var tokenType => throw new JsonException($"Could not parse json sequence starting with {tokenType} as DateTimeOffset (epoch ms)."),
        };

    public static void Write(Utf8JsonWriter writer, DateTimeOffset value)
    {
        Span<byte> buffer = stackalloc byte[24];
        value.ToUnixTimeMilliseconds().TryFormat(buffer, out var size, default, CultureInfo.InvariantCulture);
        writer.WriteStringValue(buffer[..size]);
    }

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => Read(ref reader);


    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        => Write(writer, value);
}
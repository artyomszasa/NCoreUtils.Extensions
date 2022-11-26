using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google
{
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
            "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK",
            DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern,
            DateTimeFormatInfo.InvariantInfo.SortableDateTimePattern
        };

        public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType switch
            {
                JsonTokenType.Null => default,
                JsonTokenType.String => (DateTimeOffset?)DateTimeOffset.ParseExact(reader.GetString() ?? string.Empty, _formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces),
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
}
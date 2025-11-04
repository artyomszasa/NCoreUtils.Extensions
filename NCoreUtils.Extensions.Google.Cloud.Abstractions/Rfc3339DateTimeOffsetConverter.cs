using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class Rfc3339DateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.Null => default,
            JsonTokenType.String => NonNullableRfc3339DateTimeOffsetConverter.Read(ref reader),
            var tokenType => throw new JsonException($"Unable to convert sequence starting with {tokenType} to DateTimeOffset.")
        };

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            NonNullableRfc3339DateTimeOffsetConverter.Write(writer, value.Value);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
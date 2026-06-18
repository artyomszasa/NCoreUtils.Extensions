using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail.Json;

public sealed class NullableEpochMsConverter : JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.Null => default(DateTimeOffset?),
            _ => EpochMsConverter.Read(ref reader)
        };

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value is DateTimeOffset v)
        {
            EpochMsConverter.Write(writer, v);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
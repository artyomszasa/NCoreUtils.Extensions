using System.Buffers;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class PermssiveNullableInt64Converter : JsonConverter<long?>
{
    private bool TryParseInt64(ReadOnlySpan<byte> source, out long value)
    {
        if (Utf8Parser.TryParse(source, out value, out var bytesConsumed) && bytesConsumed == source.Length)
        {
            return true;
        }
        return false;
    }

    private bool TryParseInt64(in Utf8JsonReader reader, out long value)
    {
        if (reader.HasValueSequence)
        {
            var sequence = reader.ValueSequence;
            if (sequence.Length > 64)
            {
                value = default;
                return false;
            }
            Span<byte> buffer = stackalloc byte[unchecked((int)sequence.Length)];
            sequence.CopyTo(buffer);
            return TryParseInt64(buffer, out value);
        }
        return TryParseInt64(reader.ValueSpan, out value);
    }

    public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.Null => default(long?),
            JsonTokenType.Number => reader.GetInt64(),
            JsonTokenType.String => TryParseInt64(in reader, out var i64)
                ? i64
                : throw new JsonException($"Could not parse \"{reader.GetString()}\" as long."),
            var tokenType => throw new JsonException($"Could not parse JSON sequence starting with {tokenType} as long.")
        };

    public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
    {
        if (value is long v)
        {
            writer.WriteNumberValue(v);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.Monitoring;

public class TypedValueConverter : JsonConverter<TypedValue>
{
    public override TypedValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        reader.Expect(JsonTokenType.StartObject);
        reader.ReadOrThrow();
        TypedValue res;
        if (reader.ValueTextEquals("boolValue"u8))
        {
            reader.ReadOrThrow();
            res = TypedValue.Boolean(reader.GetBoolean());
        }
        else if (reader.ValueTextEquals("int64Value"u8))
        {
            reader.ReadOrThrow();
            res = TypedValue.Integer(reader.GetInt64());
        }
        else if (reader.ValueTextEquals("doubleValue"u8))
        {
            reader.ReadOrThrow();
            res = TypedValue.Double(reader.GetDouble());
        }
        else if (reader.ValueTextEquals("stringValue"u8))
        {
            reader.ReadOrThrow();
            res = TypedValue.String(reader.GetString() ?? string.Empty);
        }
        else
        {
            throw new JsonException($"Not a valid property for TypedValue: \"{reader.GetString()}\".");
        }
        reader.ReadOrThrow();
        reader.Expect(JsonTokenType.EndObject);
        return res;
    }

    public override void Write(Utf8JsonWriter writer, TypedValue value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        switch (value.Type)
        {
            case TypedValue.ValueType.Boolean:
                writer.WriteBoolean("boolValue"u8, value.BooleanValue);
                break;
            case TypedValue.ValueType.Integer:
                writer.WriteNumber("int64Value"u8, value.IntergerValue);
                break;
            case TypedValue.ValueType.Double:
                writer.WriteNumber("doubleValue"u8, value.DoubleValue);
                break;
            case TypedValue.ValueType.String:
                writer.WriteString("stringValue"u8, value.StringValue ?? string.Empty);
                break;
            default:
                throw new InvalidOperationException("Invalid typed value.");
        }
        writer.WriteEndObject();
    }
}
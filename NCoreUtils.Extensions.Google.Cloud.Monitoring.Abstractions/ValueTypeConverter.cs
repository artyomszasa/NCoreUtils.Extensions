using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.Monitoring;

public class ValueTypeConverter : JsonConverter<TypedValue.ValueType>
{
    public override TypedValue.ValueType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        reader.Expect(JsonTokenType.String);
        if (reader.ValueTextEquals("BOOL"u8))
        {
            return TypedValue.ValueType.Boolean;
        }
        if (reader.ValueTextEquals("INT64"u8))
        {
            return TypedValue.ValueType.Integer;
        }
        if (reader.ValueTextEquals("DOUBLE"u8))
        {
            return TypedValue.ValueType.Double;
        }
        if (reader.ValueTextEquals("STRING"u8))
        {
            return TypedValue.ValueType.String;
        }
        throw new JsonException($"Invalid ValueType: \"{reader.GetString()}\".");
    }

    public override void Write(Utf8JsonWriter writer, TypedValue.ValueType value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case TypedValue.ValueType.Boolean:
                writer.WriteStringValue("BOOL"u8);
                break;
            case TypedValue.ValueType.Integer:
                writer.WriteStringValue("INT64"u8);
                break;
            case TypedValue.ValueType.Double:
                writer.WriteStringValue("DOUBLE"u8);
                break;
            case TypedValue.ValueType.String:
                writer.WriteStringValue("STRING"u8);
                break;
            default:
                throw new InvalidOperationException("Invalid ValueType.");
        }
    }
}
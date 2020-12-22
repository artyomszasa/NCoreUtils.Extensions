using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google
{
    public class PermissiveUInt64Converter : JsonConverter<ulong?>
    {
        public override ulong? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType switch
            {
                JsonTokenType.Null => default,
                JsonTokenType.String => ulong.Parse(reader.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture),
                _ => reader.GetUInt64()
            };

        public override void Write(Utf8JsonWriter writer, ulong? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                if (9007199254740991UL < value)
                {
                    writer.WriteStringValue(value.Value.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    writer.WriteNumberValue(value.Value);
                }
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
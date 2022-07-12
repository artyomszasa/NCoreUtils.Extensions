using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Internal;

public interface ILinkedProperty
{
    PropertyInfo Property { get; }

    ParameterInfo Parameter { get; }

    object? DefaultValue { get; }

    JsonEncodedText Name { get; }

    JsonConverter? Converter { get; }

    object? ReadPropertyValue(ref Utf8JsonReader reader, JsonSerializerOptions options);

    void WritePropertyValue(Utf8JsonWriter writer, object value, JsonSerializerOptions options);
}
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using NCoreUtils.JsonSerialization.Internal;

namespace NCoreUtils.JsonSerialization
{
    public class JsonImmutableListConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(IReadOnlyList<>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var elementType = typeToConvert.GetGenericArguments()[0];
            return (JsonConverter)Activator.CreateInstance(typeof(JsonImmutableListConverter<>).MakeGenericType(elementType), true);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using NCoreUtils.JsonSerialization.Internal;

namespace NCoreUtils.JsonSerialization
{
    public class JsonImmutableDictionaryConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsGenericType
                && typeToConvert.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>)
                && typeToConvert.GetGenericArguments()[0].Equals(typeof(string));
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var elementType = typeToConvert.GetGenericArguments()[1];
            return (JsonConverter)Activator.CreateInstance(typeof(JsonImmutableDictionaryConverter<>).MakeGenericType(elementType), true);
        }
    }
}
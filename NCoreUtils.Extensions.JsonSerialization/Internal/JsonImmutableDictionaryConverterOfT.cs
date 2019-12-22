using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.JsonSerialization.Internal
{
    class JsonImmutableDictionaryConverter<T> : JsonConverter<IReadOnlyDictionary<string, T>>
    {
        public override IReadOnlyDictionary<string, T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (JsonTokenType.Null == reader.TokenType)
            {
                return null;
            }
            if (JsonTokenType.StartObject != reader.TokenType)
            {
                throw new SerializationException($"Invalid json token {reader.TokenType}.");
            }
            var itemConverter = options.GetConverter(typeof(T)) as JsonConverter<T>;
            var builder = ImmutableDictionary.CreateBuilder<string, T>();
            while (reader.Read())
            {
                if (JsonTokenType.EndObject == reader.TokenType)
                {
                    break;
                }
                if (JsonTokenType.PropertyName != reader.TokenType)
                {
                    throw new SerializationException($"Invalid json token {reader.TokenType}, property name expected.");
                }
                var key = reader.GetString();
                reader.Read();
                builder.Add(key, itemConverter.Read(ref reader, typeof(T), options));
            }
            return builder.ToImmutable();
        }

        public override void Write(Utf8JsonWriter writer, IReadOnlyDictionary<string, T> value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }
            var itemConverter = options.GetConverter(typeof(T)) as JsonConverter<T>;
            writer.WriteStartObject();
            foreach (var kv in value)
            {
                writer.WritePropertyName(kv.Key);
                itemConverter.Write(writer, kv.Value, options);
            }
            writer.WriteEndObject();
        }
    }
}
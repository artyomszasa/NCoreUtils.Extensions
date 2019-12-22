using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.JsonSerialization.Internal
{
    class JsonImmutableListConverter<T> : JsonConverter<IReadOnlyList<T>>
    {
        public override IReadOnlyList<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (JsonTokenType.Null == reader.TokenType)
            {
                return null;
            }
            if (JsonTokenType.StartArray != reader.TokenType)
            {
                throw new SerializationException($"Invalid json token {reader.TokenType}.");
            }
            var itemConverter = options.GetConverter(typeof(T)) as JsonConverter<T>;
            var builder = ImmutableArray.CreateBuilder<T>();
            while (reader.Read())
            {
                if (JsonTokenType.EndArray == reader.TokenType)
                {
                    break;
                }
                builder.Add(itemConverter.Read(ref reader, typeof(T), options));
            }
            return builder.ToImmutable();
        }

        public override void Write(Utf8JsonWriter writer, IReadOnlyList<T> value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }
            var itemConverter = options.GetConverter(typeof(T)) as JsonConverter<T>;
            writer.WriteStartArray();
            foreach (var item in value)
            {
                itemConverter.Write(writer, item, options);
            }
            writer.WriteEndArray();
        }
    }
}
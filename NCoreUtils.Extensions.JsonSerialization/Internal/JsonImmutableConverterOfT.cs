using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.JsonSerialization.Internal
{
    class JsonImmutableConverter<T> : JsonConverter<T>
    {
        static object DefValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        IImmutableObjectDescriptor _descriptor;

        public JsonImmutableConverter(IImmutableObjectDescriptor descriptor)
        {
            _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (JsonTokenType.Null == reader.TokenType && !typeof(T).IsValueType)
            {
                return default;
            }
            if (JsonTokenType.StartObject != reader.TokenType)
            {
                throw new SerializationException("Must be an object.");
            }
            var values = new Dictionary<PropertyInfo, object>();
            while (reader.Read())
            {
                if (JsonTokenType.EndObject == reader.TokenType)
                {
                    break;
                }
                if (JsonTokenType.PropertyName != reader.TokenType)
                {
                    throw new InvalidOperationException("Should never happen");
                }
                var propName = reader.GetString();
                if (!_descriptor.Properties.TryGetValue(propName, out var prop))
                {
                    reader.Read();
                    if (!reader.TrySkip())
                    {
                        throw new JsonException();
                    }
                }
                else
                {
                    reader.Read();
                    values.Add(prop, GenericConverter.Read(prop.PropertyType, ref reader, options));
                }
            }
            var args = _descriptor.Ctor.GetParameters().MapToArray(par =>
            {
                if (values.TryGetValue(_descriptor.ParameterMapping[par], out var value))
                {
                    return value;
                }
                return DefValue(par.ParameterType);
            });
            return (T)_descriptor.Ctor.Invoke(args);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }
            writer.WriteStartObject();
            foreach (var property in _descriptor.Properties.Values)
            {
                var name = property.GetCustomAttribute<JsonPropertyNameAttribute>() switch
                {
                    null => null == options.PropertyNamingPolicy
                        ? property.Name
                        : options.PropertyNamingPolicy.ConvertName(property.Name),
                    { Name: var jsonName } => jsonName
                };
                writer.WritePropertyName(name);
                GenericConverter.Write(property.PropertyType, writer, property.GetValue(value, null), options);
            }
            writer.WriteEndObject();
        }
    }
}
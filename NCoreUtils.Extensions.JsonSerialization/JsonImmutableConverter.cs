using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using NCoreUtils.JsonSerialization.Internal;

namespace NCoreUtils.JsonSerialization
{
    public class JsonImmutableConverter : JsonConverterFactory
    {
        static readonly ImmutableHashSet<Type> _primitiveLike = ImmutableHashSet.CreateRange(new []
        {
            typeof(string),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(Guid)
        });

        readonly IJsonImmutableSerializerCache _cache;

        public JsonImmutableConverter(IJsonImmutableSerializerCache cache)
        {
            _cache = cache ?? DefaultJsonImmutableSerializerCache.SharedInstance;
        }

        public JsonImmutableConverter() : this(null) { }

        bool TryGetDescriptor(Type type, out IImmutableObjectDescriptor descriptor)
        {
            if (_cache.TryGetValue(type, out descriptor))
            {
                return null != descriptor;
            }
            var desc = ImmutableObjectDescriptor.TryCreateDescriptor(type, out var d) ? d : null;
            _cache[type] = desc;
            descriptor = desc;
            return null != desc;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.IsPrimitive || _primitiveLike.Contains(typeToConvert))
            {
                return false;
            }
            return TryGetDescriptor(typeToConvert, out var _);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (TryGetDescriptor(typeToConvert, out var descriptor))
            {
                return (JsonConverter)Activator.CreateInstance(typeof(JsonImmutableConverter<>).MakeGenericType(typeToConvert), new object[] { descriptor });
            }
            throw new InvalidOperationException($"Unable to create immutable object converter for {typeToConvert}.");
        }
    }
}
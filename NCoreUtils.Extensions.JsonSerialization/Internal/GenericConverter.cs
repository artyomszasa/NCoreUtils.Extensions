using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.JsonSerialization.Internal
{
    abstract class GenericConverter
    {
        static readonly ConcurrentDictionary<Type, GenericConverter> _cache = new ConcurrentDictionary<Type, GenericConverter>();

        static readonly Func<Type, GenericConverter> _factory
            = ty => (GenericConverter)Activator.CreateInstance(typeof(GenericConverter<>).MakeGenericType(ty), true);

        public static object Read(Type type, ref Utf8JsonReader reader, JsonSerializerOptions options)
            => _cache.GetOrAdd(type, _factory)
                .Read(ref reader, options);

        public static void Write(Type type, Utf8JsonWriter writer, object value, JsonSerializerOptions options)
            => _cache.GetOrAdd(type, _factory)
                .Write(writer, value, options);

        protected abstract object Read(ref Utf8JsonReader reader, JsonSerializerOptions options);

        protected abstract void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options);
    }

    sealed class GenericConverter<T> : GenericConverter
    {
        protected override object Read(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var c = (JsonConverter<T>)options.GetConverter(typeof(T));
            return c.Read(ref reader, typeof(T), options);
        }

        protected override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var c = (JsonConverter<T>)options.GetConverter(typeof(T));
            c.Write(writer, (T)value, options);
        }
    }
}
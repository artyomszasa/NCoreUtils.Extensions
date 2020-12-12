using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json.Serialization;
using NCoreUtils.Internal;

namespace NCoreUtils
{
    public static class ImmutableJsonConverterFactory
    {
        private static ConcurrentDictionary<Type, object> _cache = new ConcurrentDictionary<Type, object>();

        private static Func<Type, object> _defFactory =
            type => Activator.CreateInstance(typeof(ImmutableJsonConverter<>).MakeGenericType(type), true);

        private static object Factory(Type type, ImmutableJsonCoverterOptions options)
        {
            var ctor = typeof(ImmutableJsonConverter<>)
                .MakeGenericType(type)
                .GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new [] { typeof(ImmutableJsonCoverterOptions) }, null);
            return ctor.Invoke(new object[] { options });
        }

        public static JsonConverter<T> GetOrCreate<T>()
            => (JsonConverter<T>)_cache.GetOrAdd(typeof(T), _defFactory);

        public static JsonConverter<T> GetOrCreate<T>(Action<ImmutableJsonCoverterOptionsBuilder<T>> configure)
        {
            if (_cache.TryGetValue(typeof(T), out var converter))
            {
                return (JsonConverter<T>)converter;
            }
            var builder = new ImmutableJsonCoverterOptionsBuilder<T>();
            configure(builder);
            return (JsonConverter<T>)_cache.GetOrAdd(typeof(T), ty => Factory(ty, builder.Build()));
        }
    }
}
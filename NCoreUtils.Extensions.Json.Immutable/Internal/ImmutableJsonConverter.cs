using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using ValueBuffer = System.Collections.Generic.List<(NCoreUtils.Internal.LinkedProperty Member, object Value)>;

namespace NCoreUtils.Internal
{
    public static class ImmutableJsonConverter
    {
        private static ConcurrentDictionary<(Type, JsonNamingPolicy?, ImmutableJsonCoverterOptions), ObjectDescription> _cache
            = new ConcurrentDictionary<(Type, JsonNamingPolicy?, ImmutableJsonCoverterOptions), ObjectDescription>();

        private static Func<(Type, JsonNamingPolicy?, ImmutableJsonCoverterOptions), ObjectDescription> _factory =
            args => ObjectDescription.Create(args.Item1, args.Item2, args.Item3);

        public static void ClearObjectDescriptionCache()
            => _cache.Clear();

        public static ObjectDescription GetObjectDescription(Type type, JsonNamingPolicy? jsonNamingPolicy, ImmutableJsonCoverterOptions options)
            => _cache.GetOrAdd((type, jsonNamingPolicy, options), _factory);

        public static ObjectDescription GetObjectDescription(Type type, JsonNamingPolicy? jsonNamingPolicy)
            => GetObjectDescription(type, jsonNamingPolicy, ImmutableJsonCoverterOptions.Default);
    }

    public sealed class ImmutableJsonConverter<T> : JsonConverter<T>, IImmutableJsonConverter<T>
    {
#if NETSTANDARD2_0
        Type IImmutableJsonConverter.TargetType => typeof(T);
#endif

        public ImmutableJsonCoverterOptions Options { get; }

        public ImmutableJsonConverter(ImmutableJsonCoverterOptions options)
            => Options = options ?? ImmutableJsonCoverterOptions.Default;

        public ImmutableJsonConverter() : this(default!) { }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new InvalidOperationException($"Expected {JsonTokenType.StartObject}, found {reader.TokenType}.");
            }
            var desc = ImmutableJsonConverter.GetObjectDescription(typeof(T), options.PropertyNamingPolicy, Options);
            var buffer = new ValueBuffer(32);
            reader.Read();
            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new InvalidOperationException($"Expected {JsonTokenType.PropertyName}, found {reader.TokenType}.");
                }
                LinkedProperty? prop = default;
                foreach (var candidate in desc.Members)
                {
                    if (reader.ValueTextEquals(candidate.Name.EncodedUtf8Bytes))
                    {
                        prop = candidate;
                        break;
                    }
                }
                reader.Read();
                if (prop is null)
                {
                    if (!reader.TrySkip())
                    {
                        throw new JsonException();
                    }
                }
                else
                {
                    buffer.Add((prop, JsonSerializer.Deserialize(ref reader, prop.Parameter.ParameterType, options)));
                }
                reader.Read();
            }
            var args = new object[desc.Members.Count];
            for (var i = 0; i < desc.Members.Count; ++i)
            {
                var member = desc.Members[i];
                var found = false;
                foreach (var (m, v) in buffer)
                {
                    if (m.Equals(member))
                    {
                        args[i] = v;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    args[i] = member.DefaultValue!;
                }
            }
            return (T)desc.Ctor.Invoke(args);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var desc = ImmutableJsonConverter.GetObjectDescription(typeof(T), options.PropertyNamingPolicy, Options);
            writer.WriteStartObject();
            foreach (var member in desc.Members)
            {
                var v = member.Property.GetValue(value!, null);
                if (v != null || !options.IgnoreNullValues)
                {
                    writer.WritePropertyName(member.Name);
                    JsonSerializer.Serialize(writer, v, member.Parameter.ParameterType, options);
                }
            }
            writer.WriteEndObject();
        }
    }
}
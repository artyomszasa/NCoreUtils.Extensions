using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using ValueBuffer = System.Collections.Generic.List<(NCoreUtils.Internal.ILinkedProperty Member, object Value)>;

namespace NCoreUtils.Internal
{
    public static class ImmutableJsonConverter
    {
        private struct CacheKey : IEquatable<CacheKey>
        {
            [DynamicallyAccessedMembers(D.CtorAndProps)]
            public Type Type { get; }

            public JsonNamingPolicy? NamingPolicy { get; }

            public ImmutableJsonCoverterOptions Options { get; }

            public CacheKey(
                [DynamicallyAccessedMembers(D.CtorAndProps)] Type type,
                JsonNamingPolicy? namingPolicy,
                ImmutableJsonCoverterOptions options)
            {
                Type = type;
                NamingPolicy = namingPolicy;
                Options = options;
            }

            public bool Equals(CacheKey other)
                => Type.Equals(other.Type)
                    && NamingPolicy == other.NamingPolicy
                    && Options == other.Options;

            public override bool Equals([NotNullWhen(true)] object? obj)
                => obj is CacheKey other && Equals(other);

            public override int GetHashCode()
                => HashCode.Combine(Type, NamingPolicy, Options);
        }

        private static readonly ConcurrentDictionary<CacheKey, ObjectDescription> _cache = new();

        private static readonly Func<CacheKey, ObjectDescription> _factory =
            args => ObjectDescription.Create(args.Type, args.NamingPolicy, args.Options);

        public static void ClearObjectDescriptionCache()
            => _cache.Clear();

        public static ObjectDescription GetObjectDescription([DynamicallyAccessedMembers(D.CtorAndProps)] Type type, JsonNamingPolicy? jsonNamingPolicy, ImmutableJsonCoverterOptions options)
            => _cache.GetOrAdd(new(type, jsonNamingPolicy, options), _factory);

        public static ObjectDescription GetObjectDescription([DynamicallyAccessedMembers(D.CtorAndProps)] Type type, JsonNamingPolicy? jsonNamingPolicy)
            => GetObjectDescription(type, jsonNamingPolicy, ImmutableJsonCoverterOptions.Default);
    }

    public sealed class ImmutableJsonConverter<[DynamicallyAccessedMembers(D.CtorAndProps)] T> : JsonConverter<T>, IImmutableJsonConverter<T>
    {
        public ImmutableJsonCoverterOptions Options { get; }

        public ImmutableJsonConverter(ImmutableJsonCoverterOptions options)
            => Options = options ?? ImmutableJsonCoverterOptions.Default;

        public ImmutableJsonConverter() : this(default!) { }

        [RequiresUnreferencedCode("Types of the properties must be preserved")]
        [UnconditionalSuppressMessage("Trimming", "IL2046", Justification = "RequiresUnreferencedCode is placed on factory methods.")]
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
                ILinkedProperty? prop = default;
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
                    var value = prop.ReadPropertyValue(ref reader, options);
                    buffer.Add((prop, value!));
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

        [RequiresUnreferencedCode("Types of the properties must be preserved")]
        [UnconditionalSuppressMessage("Trimming", "IL2046", Justification = "RequiresUnreferencedCode is placed on factory methods.")]
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var desc = ImmutableJsonConverter.GetObjectDescription(typeof(T), options.PropertyNamingPolicy, Options);
            writer.WriteStartObject();
            foreach (var member in desc.Members)
            {
                var v = member.Property.GetValue(value!, null);
                if (v != null || options.DefaultIgnoreCondition == JsonIgnoreCondition.Never)
                {
                    writer.WritePropertyName(member.Name);
                    JsonSerializer.Serialize(writer, v, member.Parameter.ParameterType, options);
                }
            }
            writer.WriteEndObject();
        }
    }
}
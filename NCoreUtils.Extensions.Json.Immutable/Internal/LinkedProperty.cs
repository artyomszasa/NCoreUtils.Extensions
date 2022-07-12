using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Internal
{
    public sealed class LinkedProperty<[DynamicallyAccessedMembers(D.CtorAndProps)] T> : ILinkedProperty, IEquatable<LinkedProperty<T>>
    {
        JsonConverter? ILinkedProperty.Converter => Converter;

        public PropertyInfo Property { get; }

        public ParameterInfo Parameter { get; }

        public object? DefaultValue { get; }

        public JsonEncodedText Name { get; }

        public JsonConverter<T>? Converter { get; }

        public LinkedProperty(ParameterInfo parameter, PropertyInfo property, JsonEncodedText name, JsonConverter<T>? converter)
        {
            Property = property;
            Parameter = parameter;
            DefaultValue = parameter.GetCustomAttribute<DefaultParameterValueAttribute>() switch
            {
                null => default,
                var attr => attr.Value
            };
            Name = name;
            Converter = converter;
        }

        public bool Equals(LinkedProperty<T>? other)
            => other is not null
                && ReferenceEquals(Property, other.Property)
                && ReferenceEquals(Parameter, other.Parameter)
                && Name.Equals(other.Name);

        public override bool Equals(object? obj)
            => obj is LinkedProperty<T> other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(
                RuntimeHelpers.GetHashCode(Property),
                RuntimeHelpers.GetHashCode(Parameter),
                Name
            );

        object? ILinkedProperty.ReadPropertyValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
            => ReadPropertyValue(ref reader, options);

        void ILinkedProperty.WritePropertyValue(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "RequiresUnreferencedCode is placed on factory methods.")]
        public T? ReadPropertyValue(ref Utf8JsonReader reader, JsonSerializerOptions options) => Converter switch
        {
            null => JsonSerializer.Deserialize<T>(ref reader, options),
            var converter => converter.Read(ref reader, typeof(T), options)
        };

        [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "RequiresUnreferencedCode is placed on factory methods.")]
        public void WritePropertyValue(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            switch (Converter)
            {
                case null:
                    JsonSerializer.Serialize(writer, value, typeof(T), options);
                    break;
                case var converter:
                    converter.Write(writer, (T)value, options);
                    break;
            };
        }
    }
}
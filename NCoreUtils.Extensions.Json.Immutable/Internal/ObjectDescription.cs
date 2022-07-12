using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Internal
{
    public class ObjectDescription
    {
        [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "RequiresUnreferencedCode is placed on factory methods.")]
        private static ILinkedProperty CreateLinkedProperty(ParameterInfo parameter, PropertyInfo property, JsonEncodedText name, JsonConverter? converter)
            => (ILinkedProperty)Activator.CreateInstance(
                type: typeof(LinkedProperty<>).MakeGenericType(property.PropertyType),
                args: new object?[]
                {
                    parameter,
                    property,
                    name,
                    converter
                }
            )!;

        public static ObjectDescription Create([DynamicallyAccessedMembers(D.CtorAndProps)] Type type, JsonNamingPolicy? namingPolicy, ImmutableJsonCoverterOptions options)
        {
            if (!(type.BaseType is null || type.BaseType == typeof(object) || type.IsValueType || type.BaseType == typeof(ValueType)))
            {
                throw new InvalidOperationException($"Only not-derived types are supported (type.BaseType == {type.BaseType}).");
            }
            var ctor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)[0];
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var parameters = ctor.GetParameters();
            var members = properties
                .Where(property => !options.Ignored.Contains(property))
                .Select(CreateLinkedPropertyFor)
                .ToArray();
            Array.Sort(members, ByPositionComparer.Instance);
            return new ObjectDescription(ctor, members);

            [UnconditionalSuppressMessage("Trimming", "IL2072", Justification = "JsonConverterAttribute ha necessary ctor attributes.")]
            ILinkedProperty CreateLinkedPropertyFor(PropertyInfo property)
            {
                var parameter = parameters!.FirstOrDefault(p => StringComparer.InvariantCultureIgnoreCase.Equals(p.Name, property.Name));
                if (parameter is null)
                {
                    throw new InvalidOperationException($"No ctor parameter found for property {property}.");
                }
                var name = JsonEncodedText.Encode(property.GetCustomAttribute<JsonPropertyNameAttribute>() switch
                {
                    null => namingPolicy == null ? parameter.Name! : namingPolicy.ConvertName(parameter.Name!),
                    var attr => attr.Name
                });
                var converter = property.GetCustomAttribute<JsonConverterAttribute>() switch
                {
                    null => default,
                    var attr => attr.ConverterType switch
                    {
                        null => attr.CreateConverter(property.PropertyType),
                        var converterType => Activator.CreateInstance(converterType, nonPublic: true) switch
                        {
                            null => default,
                            JsonConverter c => c,
                            var o => throw new InvalidOperationException($"Invalid JsonConverter {o.GetType()}")
                        }
                    }
                };
                return CreateLinkedProperty(parameter, property, name, converter);
            }
        }

        public static ObjectDescription Create([DynamicallyAccessedMembers(D.CtorAndProps)] Type type, JsonNamingPolicy? namingPolicy)
            => Create(type, namingPolicy, ImmutableJsonCoverterOptions.Default);

        public Type Type => Ctor.DeclaringType!;

        public ConstructorInfo Ctor { get; }

        public IReadOnlyList<ILinkedProperty> Members { get; }

        private ObjectDescription(ConstructorInfo ctor, IReadOnlyList<ILinkedProperty> members)
        {
            Ctor = ctor ?? throw new ArgumentNullException(nameof(ctor));
            Members = members ?? throw new ArgumentNullException(nameof(members));
        }
    }
}
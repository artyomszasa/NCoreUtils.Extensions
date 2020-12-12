using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace NCoreUtils.Internal
{
    public class ObjectDescription
    {
        public static ObjectDescription Create(Type type, JsonNamingPolicy? namingPolicy, ImmutableJsonCoverterOptions options)
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
                .Select(property =>
                {
                    var parameter = parameters.FirstOrDefault(p => StringComparer.InvariantCultureIgnoreCase.Equals(p.Name, property.Name));
                    if (parameter is null)
                    {
                        throw new InvalidOperationException($"No ctor parameter found for property {property}.");
                    }
                    var name = JsonEncodedText.Encode(namingPolicy == null ? parameter.Name : namingPolicy.ConvertName(parameter.Name));
                    return new LinkedProperty(parameter, property, name);
                })
                .ToArray();
            Array.Sort(members, ByPositionComparer.Instance);
            return new ObjectDescription(ctor, members);
        }

        public static ObjectDescription Create(Type type, JsonNamingPolicy? namingPolicy)
            => Create(type, namingPolicy, ImmutableJsonCoverterOptions.Default);

        public Type Type => Ctor.DeclaringType;

        public ConstructorInfo Ctor { get; }

        public IReadOnlyList<LinkedProperty> Members { get; }

        private ObjectDescription(ConstructorInfo ctor, IReadOnlyList<LinkedProperty> members)
        {
            Ctor = ctor ?? throw new ArgumentNullException(nameof(ctor));
            Members = members ?? throw new ArgumentNullException(nameof(members));
        }
    }
}
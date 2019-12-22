using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json.Serialization;

namespace NCoreUtils.JsonSerialization.Internal
{
    public class ImmutableObjectDescriptor : IImmutableObjectDescriptor
    {
        public static bool TryCreateDescriptor(Type type, out ImmutableObjectDescriptor descriptor)
        {
            var properties = new List<PropertyInfo>(type.GetProperties(BindingFlags.Instance | BindingFlags.Public));
            var index = 0;
            while (index < properties.Count)
            {
                var property = properties[index];
                if (property.IsDefined(typeof(JsonIgnoreAttribute), true))
                {
                    properties.RemoveAt(index);
                }
                else
                {
                    ++index;
                }
            }
            return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .MaybePick(ctor =>
                {
                    var parameters = ctor.GetParameters();
                    if (parameters.Length != properties.Count)
                    {
                        return Maybe.Nothing;
                    }
                    var mapping = new Dictionary<ParameterInfo, PropertyInfo>();
                    foreach (var parameter in parameters)
                    {
                        var propertyName = parameter.GetCustomAttribute<JsonTargetPropertyAttribute>() switch
                        {
                            null => parameter.Name,
                            { PropertyName: var name } => name
                        };
                        var property = properties.Find(p => StringComparer.OrdinalIgnoreCase.Equals(propertyName, p.Name));
                        if (null == property)
                        {
                            return Maybe.Nothing;
                        }
                        mapping.Add(parameter, property);
                    }
                    return new ImmutableObjectDescriptor(ctor, mapping.ToImmutableDictionary()).Just();
                })
                .TryGetValue(out descriptor);
        }

        IReadOnlyDictionary<ParameterInfo, PropertyInfo> IImmutableObjectDescriptor.ParameterMapping => ParameterMapping;

        IReadOnlyDictionary<string, PropertyInfo> IImmutableObjectDescriptor.Properties => Properties;

        public Type Type => Ctor.DeclaringType;

        public ConstructorInfo Ctor { get; }

        public ImmutableDictionary<ParameterInfo, PropertyInfo> ParameterMapping { get; }

        public ImmutableDictionary<string, PropertyInfo> Properties { get; }

        public ImmutableObjectDescriptor(ConstructorInfo ctor, ImmutableDictionary<ParameterInfo, PropertyInfo> parameterMapping)
        {
            Ctor = ctor ?? throw new ArgumentNullException(nameof(ctor));
            ParameterMapping = parameterMapping ?? throw new ArgumentNullException(nameof(parameterMapping));
            Properties = parameterMapping.Values.ToImmutableDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
        }
    }
}
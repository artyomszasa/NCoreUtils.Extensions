using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace NCoreUtils.Internal
{
    public sealed class LinkedProperty : IEquatable<LinkedProperty>
    {
        public PropertyInfo Property { get; }

        public ParameterInfo Parameter { get; }

        public object? DefaultValue { get; }

        public JsonEncodedText Name { get; }

        // public Func<object, object> Getter { get; }

        public LinkedProperty(ParameterInfo parameter, PropertyInfo property, JsonEncodedText name)
        {
            Property = property;
            Parameter = parameter;
            DefaultValue = parameter.GetCustomAttribute<DefaultParameterValueAttribute>() switch
            {
                null => parameter.ParameterType.IsValueType
                    ? Activator.CreateInstance(parameter.ParameterType, true)
                    : default,
                var attr => attr.Value
            };
            Name = name;
            // // ****
            // var eArg = Expression.Parameter(typeof(object));
            // var eFunc = Expression.Lambda(
            //     Expression.Property(Expression.Convert(eArg, property.DeclaringType), property),
            //     eArg
            // );
            // Getter = eFunc.Compile();
        }

        public bool Equals(LinkedProperty? other)
            => other is not null
                && ReferenceEquals(Property, other.Property)
                && ReferenceEquals(Parameter, other.Parameter)
                && Name.Equals(other.Name);

        public override bool Equals(object? obj)
            => obj is LinkedProperty other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(
                RuntimeHelpers.GetHashCode(Property),
                RuntimeHelpers.GetHashCode(Parameter),
                Name
            );
    }
}
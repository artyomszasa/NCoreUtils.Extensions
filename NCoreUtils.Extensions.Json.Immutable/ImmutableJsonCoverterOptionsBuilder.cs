using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NCoreUtils.Internal;

namespace NCoreUtils
{
    public class ImmutableJsonCoverterOptionsBuilder
    {
        public List<PropertyInfo> Ignored { get; } = new List<PropertyInfo>();

        public Type TargetType { get; }

        public ImmutableJsonCoverterOptionsBuilder(Type targetType)
            => TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));

        public ImmutableJsonCoverterOptions Build()
            => new ImmutableJsonCoverterOptions(Ignored.ToArray());

        public ImmutableJsonCoverterOptionsBuilder Ignore(PropertyInfo property)
        {
            if (property.DeclaringType == TargetType || property.DeclaringType.IsAssignableFrom(TargetType))
            {
                Ignored.Add(property);
            }
            throw new InvalidOperationException(
                $"Unable to mark property {property} as ignored for {TargetType} because the property declaring type "
                + $"{property.DeclaringType} is not compatible with the {TargetType}."
            );
        }
    }

    public class ImmutableJsonCoverterOptionsBuilder<T> : ImmutableJsonCoverterOptionsBuilder
    {
        public ImmutableJsonCoverterOptionsBuilder()
            : base(typeof(T))
        { }

        public ImmutableJsonCoverterOptionsBuilder<T> Ignore<TProp>(Expression<Func<T, TProp>> selector)
        {
            if (selector.Body is MemberExpression mexpr && mexpr.Member is PropertyInfo prop)
            {
                return Ignore(prop);
            }
            throw new InvalidOperationException($"Invalid property selector: {selector}.");
        }

        public new ImmutableJsonCoverterOptionsBuilder<T> Ignore(PropertyInfo property)
        {
            base.Ignore(property);
            return this;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NCoreUtils.Internal;

namespace NCoreUtils
{
    public class ImmutableJsonCoverterOptionsBuilder<T>
    {
        public List<PropertyInfo> Ignored { get; } = new List<PropertyInfo>();

        public ImmutableJsonCoverterOptionsBuilder<T> Ignore<TProp>(Expression<Func<T, TProp>> selector)
        {
            if (selector.Body is MemberExpression mexpr && mexpr.Member is PropertyInfo prop)
            {
                Ignored.Add(prop);
                return this;
            }
            throw new InvalidOperationException($"Invalid property selector: {selector}.");
        }

        public ImmutableJsonCoverterOptions Build()
            => new ImmutableJsonCoverterOptions(Ignored.ToArray());
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NCoreUtils.Internal
{
    public class ImmutableJsonCoverterOptions
    {
        public static ImmutableJsonCoverterOptions Default { get; } = new ImmutableJsonCoverterOptions(new PropertyInfo[0]);

        public IReadOnlyList<PropertyInfo> Ignored { get; }

        public ImmutableJsonCoverterOptions(IReadOnlyList<PropertyInfo> ignored)
        {
            Ignored = ignored ?? throw new ArgumentNullException(nameof(ignored));
        }
    }
}
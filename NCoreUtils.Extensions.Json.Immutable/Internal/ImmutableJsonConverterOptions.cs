using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NCoreUtils.Internal
{
    public class ImmutableJsonCoverterOptions : IEquatable<ImmutableJsonCoverterOptions>
    {
        public static bool operator ==(ImmutableJsonCoverterOptions? a, ImmutableJsonCoverterOptions? b)
            => a is null ? b is null : a.Equals(b);

        public static bool operator !=(ImmutableJsonCoverterOptions? a, ImmutableJsonCoverterOptions? b)
            => a is null ? b is not null : !a.Equals(b);

        public static ImmutableJsonCoverterOptions Default { get; } = new ImmutableJsonCoverterOptions(Array.Empty<PropertyInfo>());

        public IReadOnlyList<PropertyInfo> Ignored { get; }

        public ImmutableJsonCoverterOptions(IReadOnlyList<PropertyInfo> ignored)
        {
            Ignored = ignored ?? throw new ArgumentNullException(nameof(ignored));
        }

        public bool Equals(ImmutableJsonCoverterOptions? other)
            => other is not null
                && new HashSet<PropertyInfo>(Ignored).SetEquals(other.Ignored);

        public override bool Equals(object? obj)
            => obj is ImmutableJsonCoverterOptions other && Equals(other);

        public override int GetHashCode()
        {
            var builder = new HashCode();
            builder.Add(Ignored.Count);
            foreach (var item in Ignored)
            {
                builder.Add(item);
            }
            return builder.ToHashCode();
        }
    }
}
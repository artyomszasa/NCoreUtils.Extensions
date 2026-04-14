using System.Collections.Immutable;

namespace NCoreUtils.ObservableProperties;

internal class TargetDescriptor(TypeDescriptor type, ImmutableArray<PropertyDescriptor> properties)
    : IEquatable<TargetDescriptor>
{
    public TypeDescriptor Type { get; } = type;

    public ImmutableArray<PropertyDescriptor> Properties { get; } = properties;

    #region equality

    public bool Equals([NotNullWhen(false)] TargetDescriptor? other)
        => other is not null
            && Type == other.Type
            && Properties.SequenceEqual(other.Properties);

    public override bool Equals([NotNullWhen(false)] object? obj)
        => Equals(obj as TargetDescriptor);

    public override int GetHashCode()
    {
        // FIXME: better hashing
        int hash = Type.GetHashCode();
        hash ^= Properties.Length;
        foreach (var property in Properties)
        {
            hash ^= property.GetHashCode();
        }
        return hash;
    }

    #endregion
}
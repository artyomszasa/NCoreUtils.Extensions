using System.Collections.Immutable;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NCoreUtils.Json;

internal struct EnumField(string name, string jsonValue)
    : IEquatable<EnumField>
{
    public string Name { get; } = name;

    public string JsonValue { get; } = jsonValue;

    public IdentifierNameSyntax IdentifierName => field ??= IdentifierName(Name);

    #region equality

    public static bool operator ==(EnumField a, EnumField b)
        => a.Equals(b);

    public static bool operator !=(EnumField a, EnumField b)
        => !a.Equals(b);

    public readonly bool Equals(EnumField other)
        => G.Eq(Name, other.Name)
            && G.Eq(JsonValue, other.JsonValue);

    public readonly override bool Equals([NotNullWhen(true)] object? obj)
        => obj is EnumField other && Equals(other);

    public readonly override int GetHashCode()
        => G.Hash(Name) ^ G.Hash(JsonValue);

    #endregion
}

internal sealed class EnumConverterTarget(string @namespace, string converterName, string enumFullName, ImmutableArray<EnumField> fields)
    : IEquatable<EnumConverterTarget>
{
    public string Namespace { get; } = @namespace;

    public string ConverterName { get; } = converterName;

    public string EnumFullName { get; } = enumFullName;

    public TypeSyntax EnumType => field ??= ParseTypeName(EnumFullName);

    public ImmutableArray<EnumField> Fields { get; } = fields;

    #region equality

    public static bool operator ==(EnumConverterTarget? a, EnumConverterTarget? b)
        => a is null
            ? b is null
            : a.Equals(b);

    public static bool operator !=(EnumConverterTarget? a, EnumConverterTarget? b)
        => a is null
            ? b is not null
            : !a.Equals(b);

    public bool Equals([NotNullWhen(true)] EnumConverterTarget? other)
        => ReferenceEquals(this, other)
            || (other is not null
                && G.Eq(Namespace, other.Namespace)
                && G.Eq(ConverterName, other.ConverterName)
                && G.Eq(EnumFullName, other.EnumFullName)
                && Fields.SequenceEqual(other.Fields));

    public override bool Equals([NotNullWhen(true)] object? obj)
        => Equals(obj as EnumConverterTarget);

    public override int GetHashCode()
        => G.Hash(EnumFullName) ^ Fields.AggregateHash();

    #endregion
}
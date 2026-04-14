using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NCoreUtils.ObservableProperties;

internal class TypeDescriptor(string @namespace, string name, string qualifiedName)
    : IEquatable<TypeDescriptor>
{
    public string Namespace { get; } = @namespace;

    public string Name { get; } = name;

    public string QualifiedName { get; } = qualifiedName;

    public TypeSyntax Syntax => field ??= ParseTypeName(QualifiedName);

    #region equality

    public static bool operator== (TypeDescriptor? a, TypeDescriptor? b)
        => a is null
            ? b is null
            : a.Equals(b);

    public static bool operator!= (TypeDescriptor? a, TypeDescriptor? b)
        => a is null
            ? b is not null
            : !a.Equals(b);

    public bool Equals([NotNullWhen(true)] TypeDescriptor? other)
        => other is not null && StringComparer.Ordinal.Equals(QualifiedName, other.QualifiedName);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => Equals(obj as TypeDescriptor);

    public override int GetHashCode()
        => StringComparer.Ordinal.GetHashCode(QualifiedName);

    #endregion
}

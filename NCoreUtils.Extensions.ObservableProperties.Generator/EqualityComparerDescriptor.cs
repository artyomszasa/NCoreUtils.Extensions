using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static NCoreUtils.ObservableProperties.SyntaxFactoryHelper;

namespace NCoreUtils.ObservableProperties;

internal class EqualityComparerDescriptor(TypeDescriptor type, string? singletonMember)
    : IEquatable<EqualityComparerDescriptor>
{
    private static ExpressionSyntax CreateAccessSyntax(TypeDescriptor type, string? singletonMember)
        => SimpleMemberAccessExpression(
            singletonMember is null or { Length: 0 }
                ? NewExpression(type.Syntax)
                : SimpleMemberAccessExpression(type.Syntax, IdentifierName(singletonMember)),
            IdentifierName("Equals")
        );

    public TypeDescriptor Type { get; } = type;

    public string? SingletonMember { get; } = singletonMember;

    public ExpressionSyntax AccessExpression => field ??= CreateAccessSyntax(Type, SingletonMember);

    #region equality

    public static bool operator== (EqualityComparerDescriptor? a, EqualityComparerDescriptor? b)
        => a is null
            ? b is null
            : a.Equals(b);

    public static bool operator!= (EqualityComparerDescriptor? a, EqualityComparerDescriptor? b)
        => a is null
            ? b is not null
            : !a.Equals(b);

    public bool Equals([NotNullWhen(true)] EqualityComparerDescriptor? other)
        => other is not null
            && Type == other.Type
            && StringComparer.Ordinal.Equals(SingletonMember, other.SingletonMember);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => Equals(obj as EqualityComparerDescriptor);

    public override int GetHashCode()
        // FIXME: better hashing
        => Type.GetHashCode() ^ (SingletonMember is null ? default : StringComparer.Ordinal.GetHashCode(SingletonMember));

    #endregion
}

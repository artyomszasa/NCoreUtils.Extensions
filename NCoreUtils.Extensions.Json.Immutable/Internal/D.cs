using System.Diagnostics.CodeAnalysis;

namespace NCoreUtils.Internal;

internal static class D
{
    public const DynamicallyAccessedMemberTypes CtorAndProps = DynamicallyAccessedMemberTypes.PublicConstructors|DynamicallyAccessedMemberTypes.PublicProperties;
}
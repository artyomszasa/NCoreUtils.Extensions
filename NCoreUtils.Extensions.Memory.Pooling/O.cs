using System.Runtime.CompilerServices;

namespace NCoreUtils.Memory.Pooling;

internal static class O
{
    public const MethodImplOptions Inline = MethodImplOptions.AggressiveInlining;

    public const MethodImplOptions Optimize =
#if NET5_0_OR_GREATER
        MethodImplOptions.AggressiveOptimization
#else
        default
#endif
    ;
}
using System.Runtime.CompilerServices;

namespace NCoreUtils.Memory.Pooling;

internal static class O
{
    public const MethodImplOptions Optimize =
#if NET6_0_OR_GREATER
        MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization
#else
        MethodImplOptions.AggressiveInlining
#endif
    ;
}
using System.Runtime.CompilerServices;

namespace NCoreUtils.Colors;

internal static class Opt
{
    public const MethodImplOptions Inline = MethodImplOptions.AggressiveInlining;

#if NETFRAMEWORK || NETSTANDARD

    public const MethodImplOptions Optimize = default;

#else

    public const MethodImplOptions Optimize = MethodImplOptions.AggressiveOptimization;

#endif

    public const MethodImplOptions InlineAndOptimize = Inline | Optimize;

    public const MethodImplOptions NoInlineOptimize = MethodImplOptions.NoInlining | Optimize;

}
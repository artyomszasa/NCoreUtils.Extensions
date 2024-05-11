using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NCoreUtils.Google;

public readonly partial struct ScopeCollection
{
    private const MethodImplOptions NoInliningAndAggressiveOptimization =
#if NET6_0_OR_GREATER
        MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization
#else
        MethodImplOptions.NoInlining
#endif
        ;

    private const MethodImplOptions AggressiveInliningAndAggressiveOptimization =
#if NET6_0_OR_GREATER
        MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization
#else
        MethodImplOptions.AggressiveInlining
#endif
        ;

    private static void ThrowIfScopesParameterIsNull([NotNull] IEnumerable<string>? scopes)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(scopes);
#else
        if (scopes is null)
        {
            throw new ArgumentNullException(nameof(scopes));
        }
#endif
    }

#if NETFRAMEWORK
    private static string[] GetSubArrayFrom(string[] source, int index)
    {
        var result = new string[source.Length - index];
        for (var i = index; i < source.Length; ++i)
        {
            result[i - index] = source[index];
        }
        return result;
    }
#else
    [MethodImpl(AggressiveInliningAndAggressiveOptimization)]
    private static string[] GetSubArrayFrom(string[] source, int index)
        => source[ index.. ];
#endif
}
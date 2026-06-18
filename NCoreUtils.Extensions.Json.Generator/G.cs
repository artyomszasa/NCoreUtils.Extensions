using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace NCoreUtils.Json;

internal static class G
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Eq(string? a, string? b)
        => StringComparer.Ordinal.Equals(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Hash(string? x)
        => x is null
            ? default
            : StringComparer.Ordinal.GetHashCode(x);

    public static int AggregateHash<T>(this ImmutableArray<T> source, IEqualityComparer<T>? comparer = default)
    {
        var hash = source.Length;
        var cmp = comparer ?? EqualityComparer<T>.Default;
        foreach (var item in source)
        {
            hash = hash * 31 + cmp.GetHashCode(item);
        }
        return hash;
    }
}
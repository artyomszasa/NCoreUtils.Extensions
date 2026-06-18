using System.Runtime.CompilerServices;

namespace NCoreUtils.Google;

internal static class S
{
#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Create(IFormatProvider? provider, ref DefaultInterpolatedStringHandler handler)
        => string.Create(provider, ref handler);
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Create(IFormatProvider? _, string value)
        => value;
#endif
}
using System.Runtime.CompilerServices;

namespace NCoreUtils.Colors;

internal static class StringFactory
{
    [MethodImpl(Opt.Inline)]
    internal static string FromSpan(ReadOnlySpan<char> source)
#if NETSTANDARD2_0 || NETFRAMEWORK
        => source.ToString();
#else
        => new(source);
#endif
}
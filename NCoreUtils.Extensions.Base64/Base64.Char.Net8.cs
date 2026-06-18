using System.Runtime.CompilerServices;

namespace NCoreUtils;

public static partial class Base64
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAsciiLetterOrDigit(char ch)
        => char.IsAsciiLetterOrDigit(ch);
}
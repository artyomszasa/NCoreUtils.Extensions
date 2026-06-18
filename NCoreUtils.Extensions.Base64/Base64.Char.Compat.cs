using System.Runtime.CompilerServices;

namespace NCoreUtils;

public static partial class Base64
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAsciiLetterOrDigit(char ch)
    {
        var uch = unchecked((uint)ch);
        return (((uch | 0x20u) - 'a') <= 'z' - 'a')
            || (uch - '0' <= '9' - '0');
    }
}
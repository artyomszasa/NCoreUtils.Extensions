using System.Runtime.CompilerServices;

namespace NCoreUtils;

public static partial class Base64
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (uint Quotient, uint Remainder) DivRem(uint left, uint right)
        => System.Math.DivRem(left, right);
}
namespace NCoreUtils.Collections;

internal static class ArrayHelper
{
    public static int MaxArrayLength =>
#if NET6_0_OR_GREATER
        System.Array.MaxLength;
#else
        int.MaxValue;
#endif

}
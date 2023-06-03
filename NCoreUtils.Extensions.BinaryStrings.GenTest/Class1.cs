namespace NCoreUtils.Extensions.BinaryStrings.GenTest;

public partial class Class1
{
    [BinaryString("xasd")]
    public static partial ReadOnlySpan<byte> Str1();

    [BinaryString("ééééé")]
    private static partial ReadOnlySpan<byte> Str2();

    [BinaryString("éddf")]
    internal static partial ReadOnlySpan<byte> Str3();
}

internal partial class Class2
{
    [BinaryString("xasd")]
    public static partial ReadOnlySpan<byte> Str1();

    [BinaryString("ééééé")]
    private static partial ReadOnlySpan<byte> Str2();

    [BinaryString("éddf")]
    internal static partial ReadOnlySpan<byte> Str3();
}

namespace NCoreUtils;

public class UnitTest1
{
    [Theory]
    // Standard/Padded
    [InlineData("I/8ODxAR", (byte[])[35, 255, 14, 15, 16, 17])]
    [InlineData("I/8ODxA=", (byte[])[35, 255, 14, 15, 16])]
    [InlineData("I/8ODw==", (byte[])[35, 255, 14, 15])]
    // Standard/NotPadded
    [InlineData("I/8ODxA", (byte[])[35, 255, 14, 15, 16])]
    [InlineData("I/8ODw", (byte[])[35, 255, 14, 15])]
    // URL/Padded
    [InlineData("I_8ODxAR", (byte[])[35, 255, 14, 15, 16, 17])]
    [InlineData("I_8ODxA=", (byte[])[35, 255, 14, 15, 16])]
    [InlineData("I_8ODw==", (byte[])[35, 255, 14, 15])]
    // URL/NotPadded
    [InlineData("I_8ODxA", (byte[])[35, 255, 14, 15, 16])]
    [InlineData("I_8ODw", (byte[])[35, 255, 14, 15])]
    public void DecodeSuccess(string input, byte[] expected)
    {
        var buffer = new byte[expected.Length];
        Assert.True(Base64.TryDecode(input, buffer, out var size));
        Assert.Equal(expected.Length, size);
        Assert.Equal(expected, buffer);
    }

    [Theory]
    // Standard/Padded
    [InlineData("I/8ODxAR", (byte[])[35, 255, 14, 15, 16, 17], Base64Alphabet.Standard, true)]
    [InlineData("I/8ODxA=", (byte[])[35, 255, 14, 15, 16], Base64Alphabet.Standard, true)]
    [InlineData("I/8ODw==", (byte[])[35, 255, 14, 15], Base64Alphabet.Standard, true)]
    // Standard/NotPadded
    [InlineData("I/8ODxAR", (byte[])[35, 255, 14, 15, 16, 17], Base64Alphabet.Standard, false)]
    [InlineData("I/8ODxA", (byte[])[35, 255, 14, 15, 16], Base64Alphabet.Standard, false)]
    [InlineData("I/8ODw", (byte[])[35, 255, 14, 15], Base64Alphabet.Standard, false)]
    // URL/Padded
    [InlineData("I_8ODxAR", (byte[])[35, 255, 14, 15, 16, 17], Base64Alphabet.Url, true)]
    [InlineData("I_8ODxA=", (byte[])[35, 255, 14, 15, 16], Base64Alphabet.Url, true)]
    [InlineData("I_8ODw==", (byte[])[35, 255, 14, 15], Base64Alphabet.Url, true)]
    // URL/NotPadded
    [InlineData("I_8ODxAR", (byte[])[35, 255, 14, 15, 16, 17], Base64Alphabet.Url, false)]
    [InlineData("I_8ODxA", (byte[])[35, 255, 14, 15, 16], Base64Alphabet.Url, false)]
    [InlineData("I_8ODw", (byte[])[35, 255, 14, 15], Base64Alphabet.Url, false)]
    public void EncodeSuccess(string expected, byte[] input, Base64Alphabet alphabet, bool pad)
    {
        var buffer = new char[expected.Length];
        Assert.True(Base64.TryEncode(input, buffer, out var written, alphabet, pad));
        Assert.Equal(expected.Length,  written);
        Assert.Equal(expected, new string(buffer));
    }
}

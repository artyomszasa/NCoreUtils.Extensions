using System.Buffers;
using System.Text;
using NCoreUtils.Internal;

namespace NCoreUtils;

public static class HttpHeaderHelpers
{
    private static readonly UTF8Encoding s_utf8 = new(false);

    // attr-char = ALPHA / DIGIT / "!" / "#" / "$" / "&" / "+" / "-" / "." / "^" / "_" / "`" / "|" / "~"
    //      ; token except ( "*" / "'" / "%" )
    private static readonly SearchValues<byte> s_rfc5987AttrBytes =
        SearchValues.Create("!#$&+-.0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ^_`abcdefghijklmnopqrstuvwxyz|~"u8);

    /// <summary>Transforms an ASCII character into its hexadecimal representation, adding the characters to a StringBuilder.</summary>
    private static void AddHexEscaped(byte c, ref ValueStringBuilder destination)
    {
        destination.Append('%');
        var (hi, lo) = Hex.ToHex(c);
        destination.Append(hi);
        destination.Append(lo);
    }

    public static string Encode5987(string input)
    {
        var builder = new ValueStringBuilder(stackalloc char[256]);
        byte[] utf8bytes = ArrayPool<byte>.Shared.Rent(s_utf8.GetMaxByteCount(input.Length));
        int utf8length = s_utf8.GetBytes(input, 0, input.Length, utf8bytes, 0);

        builder.Append("utf-8\'\'");

        ReadOnlySpan<byte> utf8 = utf8bytes.AsSpan(0, utf8length);
        do
        {
            int length = utf8.IndexOfAnyExcept(s_rfc5987AttrBytes);
            if (length < 0)
            {
                length = utf8.Length;
            }

            Encoding.ASCII.GetChars(utf8[..length], builder.AppendSpan(length));

            utf8 = utf8[length..];

            if (utf8.IsEmpty)
            {
                break;
            }

            length = utf8.IndexOfAny(s_rfc5987AttrBytes);
            if (length < 0)
            {
                length = utf8.Length;
            }

            foreach (byte b in utf8[..length])
            {
                AddHexEscaped(b, ref builder);
            }

            utf8 = utf8[length..];
        }
        while (!utf8.IsEmpty);

        ArrayPool<byte>.Shared.Return(utf8bytes);

        return builder.ToString();
    }
}
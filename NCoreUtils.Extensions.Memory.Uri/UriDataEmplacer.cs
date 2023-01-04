using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NCoreUtils;

public static class UriDataEmplacer
{
    private static readonly string _escaped = "%00%01%02%03%04%05%06%07%08%09%0A%0B%0C%0D%0E%0F%10%11%12%13%14%15%16%17%18%19%1A%1B%1C%1D%1E%1F%20%21%22%23%24%25%26%27%28%29%2A%2B%2C%2D%2E%2F%30%31%32%33%34%35%36%37%38%39%3A%3B%3C%3D%3E%3F%40%41%42%43%44%45%46%47%48%49%4A%4B%4C%4D%4E%4F%50%51%52%53%54%55%56%57%58%59%5A%5B%5C%5D%5E%5F%60%61%62%63%64%65%66%67%68%69%6A%6B%6C%6D%6E%6F%70%71%72%73%74%75%76%77%78%79%7A%7B%7C%7D%7E%7F%80%81%82%83%84%85%86%87%88%89%8A%8B%8C%8D%8E%8F%90%91%92%93%94%95%96%97%98%99%9A%9B%9C%9D%9E%9F%A0%A1%A2%A3%A4%A5%A6%A7%A8%A9%AA%AB%AC%AD%AE%AF%B0%B1%B2%B3%B4%B5%B6%B7%B8%B9%BA%BB%BC%BD%BE%BF%C0%C1%C2%C3%C4%C5%C6%C7%C8%C9%CA%CB%CC%CD%CE%CF%D0%D1%D2%D3%D4%D5%D6%D7%D8%D9%DA%DB%DC%DD%DE%DF%E0%E1%E2%E3%E4%E5%E6%E7%E8%E9%EA%EB%EC%ED%EE%EF%F0%F1%F2%F3%F4%F5%F6%F7%F8%F9%FA%FB%FC%FD%FE%FF";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSafeChar(int ch)
        => ('A' <= ch && ch <= 'Z')
            || ('a' <= ch && ch <= 'z')
            || ('0' <= ch && ch <= '9')
            || ch == '-'
            || ch == '_'
            || ch == '.'
            || ch == '~';

    public static bool TryEmplaceUriEscaped(scoped ReadOnlySpan<char> source, scoped Span<char> span, out int total)
    {
        var offset = 0;
        var available = span.Length;
        foreach (var rune in source.EnumerateRunes())
        {
            if (IsSafeChar(rune.Value))
            {
                if (--available <= 0)
                {
                    total = default;
                    return false;
                }
                span[offset++] = unchecked((char)rune.Value);
            }
            else
            {
                uint utf8Byte1;
                uint utf8Byte2;
                uint utf8Byte3;
                uint utf8Byte4;
                var uch = unchecked((uint)rune.Value);
                if (uch <= 0x07FU)
                {
                    // ASCII
                    if ((available -= 3) <= 0)
                    {
                        total = default;
                        return false;
                    }
                    _escaped.AsSpan(unchecked((int)(uch * 3)), 3).CopyTo(span[offset..]);
                    offset += 3;
                }
                else if (uch <= 0x07FFU)
                {
                    // 2-byte UTF-8
                    if ((available -= 6) <= 0)
                    {
                        total = default;
                        return false;
                    }
                    utf8Byte1 = unchecked(0b11000000 | (uch >>> 6));
                    utf8Byte2 = unchecked(0x80 | (uch & 0x03F));
                    _escaped.AsSpan(unchecked((int)(utf8Byte1 * 3)), 3).CopyTo(span[offset..]);
                    offset += 3;
                    _escaped.AsSpan(unchecked((int)(utf8Byte2 * 3)), 3).CopyTo(span[offset..]);
                    offset += 3;
                }
                else if (uch <= 0x0FFFFU)
                {
                    // 3-byte UTF-8
                    if ((available -= 9) <= 0)
                    {
                        total = default;
                        return false;
                    }
                    utf8Byte1 = unchecked(0b11100000 | (uch >>> 12));
                    utf8Byte2 = unchecked(0x80 | ((uch >>> 6) & 0x03F));
                    utf8Byte3 = unchecked(0x80 | (uch & 0x03F));
                    _escaped.AsSpan(unchecked((int)(utf8Byte1 * 3)), 3).CopyTo(span[offset..]);
                    offset += 3;
                    _escaped.AsSpan(unchecked((int)(utf8Byte2 * 3)), 3).CopyTo(span[offset..]);
                    offset += 3;
                    _escaped.AsSpan(unchecked((int)(utf8Byte3 * 3)), 3).CopyTo(span[offset..]);
                    offset += 3;
                }
                else
                {
                    // 4-byte UTF-8
                    if ((available -= 12) <= 0)
                    {
                        total = default;
                        return false;
                    }
                    utf8Byte1 = unchecked((0b11110000U | (uch >>> 18)) & 0x0FFU);
                    utf8Byte2 = unchecked(0x80U | ((uch >>> 12) & 0x03FU));
                    utf8Byte3 = unchecked(0x80U | ((uch >>> 6) & 0x03FU));
                    utf8Byte4 = unchecked(0x80U | (uch & 0x03FU));
                    _escaped.AsSpan(unchecked((int)(utf8Byte1 * 3)), 3).CopyTo(span[offset..]);
                    offset += 3;
                    _escaped.AsSpan(unchecked((int)(utf8Byte2 * 3)), 3).CopyTo(span[offset..]);
                    offset += 3;
                    _escaped.AsSpan(unchecked((int)(utf8Byte3 * 3)), 3).CopyTo(span[offset..]);
                    offset += 3;
                    _escaped.AsSpan(unchecked((int)(utf8Byte4 * 3)), 3).CopyTo(span[offset..]);
                    offset += 3;
                }
            }
        }
        total = offset;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int EmplaceUriEscaped(ReadOnlySpan<char> source, Span<char> span)
    {
        if (TryEmplaceUriEscaped(source, span, out var length))
        {
            return length;
        }
        throw new InsufficientBufferSizeException(span);
    }

    [return: NotNullIfNotNull(nameof(source))]
    public static string? ToUriEscapedString(this string? source)
    {
        if (source is null)
        {
            return default;
        }
        if (source.Length == 0)
        {
            return source;
        }
        var buffer = ArrayPool<char>.Shared.Rent(source.Length * 12);
        try
        {
            var size = EmplaceUriEscaped(source, buffer);
            return new string(buffer, 0, size);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }


}
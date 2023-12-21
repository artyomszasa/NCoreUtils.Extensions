using System;

namespace NCoreUtils.Internal;

internal static class LineRange
{
    public static void GetLineRange(ReadOnlySpan<char> source, out (int Start, int End) range, out int used)
    {
        var l = source.Length;
        var i = 0;
        // skip '\r'-s
        while (i < l && source[i] == '\r') { ++i; }
        if (i == l)
        {
            range = (0, 0);
            used = i;
            return;
        }
        var start = i;
        while (i < l && source[i] != '\n') { ++i; }
        var end = i;
        if (i != l)
        {
            ++i;
        }
        // exclude '\r'-s before '\n'
        while (end > start && source[end - 1] == '\r') { --end; }
        range = (start, end);
        used = i;
    }
}
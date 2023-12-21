using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NCoreUtils.Internal;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public sealed class TrimmedLineEnumerator(string source) : IEnumerator<string>
{
    private string Source { get; } = source ?? throw new ArgumentNullException(nameof(source));

    private int Offset { get; set; }

    private bool TrailingEmptyLine { get; set; } = source.Length == 0 || source[^1] == '\n';

    object IEnumerator.Current
    {
        [ExcludeFromCodeCoverage]
        get => Current;
    }

    public string Current { get; private set; } = string.Empty;

    [ExcludeFromCodeCoverage]
    void IEnumerator.Reset()
        => Offset = 0;

    public bool MoveNext()
    {
        if (Offset == Source.Length)
        {
            Current = string.Empty;
            if (TrailingEmptyLine)
            {
                TrailingEmptyLine = false;
                return true;
            }
            return false;
        }
        LineRange.GetLineRange(Source.AsSpan()[Offset..], out var range, out var used);
        var span = Source.AsSpan();
        var (start, end) = range;
        start += Offset;
        end += Offset;
        while (start < end && char.IsWhiteSpace(span[start])) { ++start; }
        while (start < end && char.IsWhiteSpace(span[end - 1])) { --end; }
        Current = Source[start..end];
        Offset += used;
        return true;
    }

    void IDisposable.Dispose() { /* noop */ }
}
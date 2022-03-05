using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NCoreUtils.Internal;

public sealed class LineEnumerator : IEnumerator<string>
{
    private string Source { get; }

    private int Offset { get; set; }

    private bool TrailingEmptyLine { get; set; }

    object IEnumerator.Current
    {
        [ExcludeFromCodeCoverage]
        get => Current;
    }

    public string Current { get; private set; } = string.Empty;

    public LineEnumerator(string source)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        TrailingEmptyLine = source.Length == 0 || source[^1] == '\n';
    }

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
        Current = Source[(Offset + range.Start)..(Offset + range.End)];
        Offset += used;
        return true;
    }

    void IDisposable.Dispose() { /* noop */ }
}
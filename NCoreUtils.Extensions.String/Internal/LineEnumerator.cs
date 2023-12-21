using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NCoreUtils.Internal;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public sealed class LineEnumerator(string source) : IEnumerator<string>
{
    private readonly string _source = source ?? throw new ArgumentNullException(nameof(source));

    private int _offset;

    private bool _trailingEmptyLine = source.Length == 0 || source[^1] == '\n';

    private string _current = string.Empty;

    object IEnumerator.Current
    {
        [ExcludeFromCodeCoverage]
        get => Current;
    }

    public string Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _current;
    }

    [ExcludeFromCodeCoverage]
    void IEnumerator.Reset()
        => _offset = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        if (_offset == _source.Length)
        {
            _current = string.Empty;
            if (_trailingEmptyLine)
            {
                _trailingEmptyLine = false;
                return true;
            }
            return false;
        }
        LineRange.GetLineRange(_source.AsSpan()[_offset..], out var range, out var used);
        _current = _source[(_offset + range.Start)..(_offset + range.End)];
        _offset += used;
        return true;
    }

    void IDisposable.Dispose() { /* noop */ }
}
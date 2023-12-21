using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NCoreUtils.Internal;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public sealed class LineEnumerable(string source) : IEnumerable<string>
{
    public string Source { get; } = source ?? throw new ArgumentNullException(nameof(source));

    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<string> GetEnumerator()
        => new LineEnumerator(Source);
}
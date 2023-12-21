using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NCoreUtils.Internal;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public sealed class TrimmedLineEnumerable(string source) : IEnumerable<string>
{
    public string Source { get; } = source ?? throw new ArgumentNullException(nameof(source));

    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public IEnumerator<string> GetEnumerator()
        => new TrimmedLineEnumerator(Source);
}
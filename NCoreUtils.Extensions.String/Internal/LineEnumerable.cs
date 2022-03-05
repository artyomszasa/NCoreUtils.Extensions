using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NCoreUtils.Internal;

public class LineEnumerable : IEnumerable<string>
{
    public string Source { get; }

    public LineEnumerable(string source)
        => Source = source ?? throw new ArgumentNullException(nameof(source));

    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public IEnumerator<string> GetEnumerator()
        => new LineEnumerator(Source);
}
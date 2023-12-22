using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace NCoreUtils.ObservableProperties;

internal static class StringExtensions
{
    [return: NotNullIfNotNull(nameof(source))]
    public static string? Capitalize(this string? source)
    {
        if (source is null || source.Length < 1)
        {
            return source;
        }
        var orig = source[0];
        var upper = char.ToUpperInvariant(orig);
        if (orig != upper)
        {
            var buffer = ArrayPool<char>.Shared.Rent(source.Length);
            try
            {
                var span = buffer.AsSpan(0, source.Length);
                source.AsSpan().CopyTo(span);
                span[0] = upper;
                return span.ToString();
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
        }
        return source;
    }
}
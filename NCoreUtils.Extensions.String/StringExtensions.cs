using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using NCoreUtils.Internal;

namespace NCoreUtils;

public static class StringExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull("placeholder")]
    public static string? Supply(this string? source, string? placeholder)
        => string.IsNullOrEmpty(source) ? placeholder : source;

    private static IEnumerable<string> SkipEmpty(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            if (!string.IsNullOrEmpty(line))
            {
                yield return line;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
        => string.IsNullOrEmpty(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEmpty([NotNullWhen(true)] this string? value)
        => !value.IsNullOrEmpty();

    public static IEnumerable<string> SplitIntoLines(this string? source, StringSplitOptions options)
    {
        if (source is null)
        {
            return Enumerable.Empty<string>();
        }
#if NET6_0_OR_GREATER
        var lines = options.HasFlag(StringSplitOptions.TrimEntries)
            ? (IEnumerable<string>)new TrimmedLineEnumerable(source)
            : new LineEnumerable(source);
#else
        var lines = new LineEnumerable(source);
#endif
        return options.HasFlag(StringSplitOptions.RemoveEmptyEntries)
            ? SkipEmpty(lines)
            : lines;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<string> SplitIntoLines(this string? source)
        => source is null ? Enumerable.Empty<string>() : new LineEnumerable(source);
}
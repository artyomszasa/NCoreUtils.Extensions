using System;
using System.Runtime.CompilerServices;

namespace NCoreUtils;

public static class SpanBuilderUriEscapeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendUriEscaped(this scoped ref SpanBuilder builder, scoped ReadOnlySpan<char> source)
    {
        builder.Length += UriDataEmplacer.EmplaceUriEscaped(source, builder.Reminder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryAppendUriEscaped(this scoped ref SpanBuilder builder, scoped ReadOnlySpan<char> source)
    {
        if (UriDataEmplacer.TryEmplaceUriEscaped(source, builder.Reminder, out var used))
        {
            builder.Length += used;
            return true;
        }
        return false;
    }
}
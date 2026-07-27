using System;
using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using NCoreUtils.Memory;

namespace NCoreUtils;

public static class SpanBuilderUriEscapeExtensions
{
    private const int MaxStackAllocSize = 64;

    private const int MaxPooledBufferSize = 512;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CheckEmplacedSize(int requestedSize, int actualSize)
    {
        if (requestedSize != actualSize)
        {
            throw new InvalidOperationException(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"source.GetEmplaceBufferSize() returned {requestedSize} but source.Emplace(..) returned {actualSize}."
                )
            );
        }
    }

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

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool TryAppendUriEscaped(
        this scoped ref SpanBuilder builder,
        ISpanExactEmplaceable source,
        ArrayPool<char>? pool = default,
        int? maxPooledBufferSize = default,
        bool fallbackToString = false)
    {
        var requiredBufferSize = source.GetEmplaceBufferSize();
        if (requiredBufferSize <= 64)
        {
            Span<char> stackBuffer = stackalloc char[requiredBufferSize];
            var size = source.Emplace(stackBuffer);
            CheckEmplacedSize(requiredBufferSize, size);
            return builder.TryAppendUriEscaped(stackBuffer);
        }
        if (requiredBufferSize <= (maxPooledBufferSize ?? MaxPooledBufferSize))
        {
            var arrayPool = pool ?? ArrayPool<char>.Shared;
            var pooledBuffer = arrayPool.Rent(requiredBufferSize);
            try
            {
                var size = source.Emplace(pooledBuffer);
                CheckEmplacedSize(requiredBufferSize, size);
                return builder.TryAppendUriEscaped(pooledBuffer);
            }
            finally
            {
                arrayPool.Return(pooledBuffer);
            }
        }
        return fallbackToString && builder.TryAppendUriEscaped(source.ToString() ?? string.Empty);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool TryAppendUriEscaped(
        this scoped ref SpanBuilder builder,
        ISpanFormattable source,
        ReadOnlySpan<char> format = default,
        IFormatProvider? formatProvider = default,
        ArrayPool<char>? pool = default,
        int? maxPooledBufferSize = default,
        bool fallbackToString = false)
    {
        // try on stack
        {
            Span<char> stackBuffer = stackalloc char[MaxStackAllocSize];
            if (source.TryFormat(stackBuffer, out var size, format, formatProvider))
            {
                return builder.TryAppendUriEscaped(stackBuffer);
            }
        }
        // try using pooled buffer
        {
            var arrayPool = pool ?? ArrayPool<char>.Shared;
            var pooledBuffer = arrayPool.Rent(maxPooledBufferSize ?? MaxPooledBufferSize);
            try
            {
                if (source.TryFormat(pooledBuffer, out var size, format, formatProvider))
                {
                    return builder.TryAppendUriEscaped(pooledBuffer);
                }
            }
            finally
            {
                arrayPool.Return(pooledBuffer);
            }
        }
        // fallback or fail
        return fallbackToString && builder.TryAppendUriEscaped(source.ToString(new string(format), formatProvider) ?? string.Empty);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void AppendUriEscaped(
        this scoped ref SpanBuilder builder,
        ISpanExactEmplaceable source,
        ArrayPool<char>? pool = default,
        int? maxPooledBufferSize = default,
        bool fallbackToString = false)
    {
        var requiredBufferSize = source.GetEmplaceBufferSize();
        if (requiredBufferSize <= 64)
        {
            Span<char> stackBuffer = stackalloc char[requiredBufferSize];
            var size = source.Emplace(stackBuffer);
            CheckEmplacedSize(requiredBufferSize, size);
            builder.AppendUriEscaped(stackBuffer);
        }
        else if (requiredBufferSize <= (maxPooledBufferSize ?? MaxPooledBufferSize))
        {
            var arrayPool = pool ?? ArrayPool<char>.Shared;
            var pooledBuffer = arrayPool.Rent(requiredBufferSize);
            try
            {
                var size = source.Emplace(pooledBuffer);
                CheckEmplacedSize(requiredBufferSize, size);
                builder.AppendUriEscaped(pooledBuffer);
            }
            finally
            {
                arrayPool.Return(pooledBuffer);
            }
        }
        else if (fallbackToString)
        {
            builder.AppendUriEscaped(source.ToString() ?? string.Empty);
        }
        else
        {
            throw new InvalidOperationException("Source formatting would require using ToString().");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void AppendUriEscaped(
        this scoped ref SpanBuilder builder,
        ISpanFormattable source,
        ReadOnlySpan<char> format = default,
        IFormatProvider? formatProvider = default,
        ArrayPool<char>? pool = default,
        int? maxPooledBufferSize = default,
        bool fallbackToString = false)
    {
        // try on stack
        {
            Span<char> stackBuffer = stackalloc char[MaxStackAllocSize];
            if (source.TryFormat(stackBuffer, out var size, format, formatProvider))
            {
                builder.AppendUriEscaped(stackBuffer);
                return;
            }
        }
        // try using pooled buffer
        {
            var arrayPool = pool ?? ArrayPool<char>.Shared;
            var pooledBuffer = arrayPool.Rent(maxPooledBufferSize ?? MaxPooledBufferSize);
            try
            {
                if (source.TryFormat(pooledBuffer, out var size, format, formatProvider))
                {
                    builder.AppendUriEscaped(pooledBuffer);
                    return;
                }
            }
            finally
            {
                arrayPool.Return(pooledBuffer);
            }
        }
        // fallback or fail
        if (fallbackToString)
        {
            builder.AppendUriEscaped(source.ToString(new string(format), formatProvider) ?? string.Empty);
        }
        else
        {
            throw new InvalidOperationException("Source formatting would require using ToString().");
        }
    }
}
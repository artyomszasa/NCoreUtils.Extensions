using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NCoreUtils;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly ref struct SpanOwner<T>(ArrayPool<T> pool, int length)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Span<T>(SpanOwner<T> owner)
        => owner.Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<T>(SpanOwner<T> owner)
        => owner.Span;

    private readonly T[] _array = pool.Rent(length);

    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => length;
    }

    public readonly Span<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
#if NET6_0_OR_GREATER
            return System.Runtime.InteropServices.MemoryMarshal.CreateSpan(
                ref System.Runtime.InteropServices.MemoryMarshal.GetArrayDataReference(_array),
                length
            );
#else
            return new(_array, 0, length);
#endif
        }
    }

    public readonly ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Preconditions.ThrowIfNegative(index);
            Preconditions.ThrowIfGreaterThan(index, Length - 1);
#if NET6_0_OR_GREATER
            return ref Unsafe.Add(ref System.Runtime.InteropServices.MemoryMarshal.GetArrayDataReference(_array), index);
#else
            return ref _array[index];
#endif
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] DangerousGetArray()
        => _array;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T>.Enumerator GetEnumerator()
        => Span.GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<T> Slice(int start, int length)
#if NET6_0_OR_GREATER
    {
        Preconditions.ThrowIfNegative(start);
        Preconditions.ThrowIfNegative(length);
        Preconditions.ThrowIfGreaterThan(length, Length - start);
        return System.Runtime.InteropServices.MemoryMarshal.CreateSpan(
            ref Unsafe.Add(ref System.Runtime.InteropServices.MemoryMarshal.GetArrayDataReference(_array), start),
            length
        );
    }
#else
        => new(_array, start, length);
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
        => pool.Return(_array);
}

public static class SpanOwner
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SharedSpanOwner<T> Allocate<T>(int length) => new(length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpanOwner<T> Allocate<T>(ArrayPool<T> pool, int length) => new(pool, length);
}
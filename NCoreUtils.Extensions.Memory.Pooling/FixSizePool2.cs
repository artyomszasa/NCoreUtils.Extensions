using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using NCoreUtils.Memory.Pooling;

namespace NCoreUtils;

public class FixSizePool2<T>
{
    private readonly T?[] _items;

    private readonly uint _maxIndex;

    private Ix _start;

    private Ix _end;

    public int AvailableCount
    {
        [MethodImpl(O.Inline)]
        get => Ix.ComputeSize(
            start: in _start,
            end: in _end,
            capacity: unchecked((uint)_items.Length)
        );
    }

    public FixSizePool2(int size)
    {
        if (size < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }
        _items = new T[size];
        _maxIndex = unchecked((uint)(size - 1));
    }

    // NOTE: UNIT TEST ONLY
    internal void UnsafeReset()
    {
        for (var i = 0; i < _items.Length; ++i)
        {
            _items[i] = default!;
        }
    }

    [MethodImpl(O.Inline)]
    private T? UnsafeItem(uint index)
    {
#if NET6_0_OR_GREATER
        return Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_items), index);
#else
        return _items[index];
#endif
    }

    [MethodImpl(O.Inline)]
    private void UnsafeItem(uint index, T? item)
    {
#if NET6_0_OR_GREATER
        Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_items), index) = item;
#else
        _items[index] = item;
#endif
    }

    [MethodImpl(O.Optimize | O.Inline)]
    private bool TryRentShort([MaybeNullWhen(false)] out T item)
    {
        var actualStart = _start; // relaxed copy
        // NOTE: both indices are monotonically increasing --> if they are not equal then start is lower than end.
        if (!Ix.EqLoopInline(actualStart, /* relaxed access */ in _end))
        {
            var sv = Ix.GetValueInline(actualStart);
            Debug.Assert(sv >= 0 && sv < _items.Length, "index out of range");
            var candidate = UnsafeItem(sv);
            var newStart = sv == _maxIndex
                ? Ix.ToggleLoopInline(actualStart)
                : Ix.Inc(actualStart);
            // NOTE: synchronization happens here --> if relaxed memory acquire resulted in outdated value the operation
            // will fail.
            if (actualStart.Equals(Ix.CompareExchange(ref _start, newStart, actualStart)))
            {
                // operation has succeeded
                item = candidate!;
                return true;
            }
        }
        item = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.NoInlining | O.Optimize)]
    private bool TryRentLong([MaybeNullWhen(false)] out T item)
    {
        while (true)
        {
            var actualEnd = Ix.Load(in _end);
            // check if pool is not locked (no operation is in progress)
            if (!Ix.IsLockedInline(actualEnd))
            {
                var actualStart = Ix.Load(_start);
                if (actualStart.Equals(actualEnd))
                {
                    // no items avaliable
                    item = default;
                    return false;
                }
                // preload item --> it will be returned if the operation succeeds
                var actualStartValue = Ix.GetValueInline(actualStart);
                var candidate = _items[actualStartValue];
                Ix newStart = actualStartValue == _maxIndex
                    ? Ix.ToggleLoopInline(actualStart)
                    : Ix.IncInline(actualStart);
                if (actualStart.Equals(Ix.CompareExchange(ref _start, newStart, actualStart)))
                {
                    // operation has succeeded
                    item = candidate!;
                    return true;
                }
            }
            // in all other cases --> operation should be retried
        }
    }

    protected virtual void Cleanup(T item) { /* noop */ }

    [MethodImpl(MethodImplOptions.NoInlining | O.Optimize)]
    public bool TryRent([MaybeNullWhen(false)] out T item)
        => TryRentShort(out item) || TryRentLong(out item);

    [MethodImpl(O.Optimize)]
    public void Return(T item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        bool success;
        do
        {
            var actualEnd = Ix.Load(in _end);
            // check if pool is not locked (no operation is in progress)
            if (!Ix.IsLockedInline(actualEnd))
            {
                var actualStart = Ix.Load(in _start);
                var capacity = _items.Length;
                if (capacity == Ix.ComputeSizeInline(actualStart, actualEnd, unchecked((uint)capacity)))
                {
                    // recheck whether start/end has changed
                    if (!actualEnd.InterlockedEquals(in _end) || !actualStart.InterlockedEquals(in _start))
                    {
                        // if so --> retry
                        success = false;
                        continue;
                    }
                    // otherwise allow GC to claim an item (default impl) or perform overridden cleanup
                    Cleanup(item);
                    return;
                }
                var newEnd = Ix.GetValueInline(actualEnd) == _maxIndex
                    ? Ix.ToggleLoopInline(actualEnd)
                    : Ix.IncInline(actualEnd);
                // Two step value application:
                // If first step succeedes --> pool is in locked state and the item can be stored safely
                // second step --> pool is unlocked
                if (Ix.TryLock(ref _end, actualEnd, newEnd, out var maskedEnd))
                {
                    // pool is locked --> proceed to store value and unlock pool
                    UnsafeItem(Ix.GetValueInline(actualEnd), item); // _items[Ix.GetValueInline(actualEnd)] = item;
                    var xres = Ix.CompareExchange(ref _end, newEnd, maskedEnd); // should always succeed
                    Debug.Assert(xres.Equals(maskedEnd), "Unlock end failed!");
                    success = true;
                }
                else
                {
                    // update has failed --> retry operation
                    success = false;
                }
            }
            else
            {
                // pool is locked (operation is in progress) --> retry current operation
                success = false;
            }
        }
        while (!success);
    }
}
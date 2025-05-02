using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NCoreUtils.Memory.Pooling;

namespace NCoreUtils;

public class FixSizePool<T>
    : IObjectPool<T>
    where T : class
{
    private readonly T?[] _items;

    private readonly uint _maxIndex;

    private Ixi _start;

    private Ixi _end;

    public int AvailableCount
    {
        [MethodImpl(O.Inline)]
        get => Ixx.ComputeSize(
            start: _start,
            end: _end,
            capacity: unchecked((uint)_items.Length)
        );
    }

    public FixSizePool(int size)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);
#else
        if (size < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }
#endif
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
        var actualStart = _start; // out-of-order copy
        // NOTE: both indices are monotonically increasing --> if they are not equal then start is lower than end.
        if (!actualStart.Eq(_end.LoadInterlocked()))
        {
            var sv = actualStart.Value;
#if DEBUG
            Debug.Assert(sv >= 0 && sv < _items.Length, "index out of range");
#endif
            var candidate = UnsafeItem(sv); // NOTE: force no bound checks
            var newStart = sv == _maxIndex
                ? actualStart.ToggleLoop()
                : actualStart.Inc();
            // NOTE: synchronization happens here --> if relaxed memory acquire resulted in outdated value the operation
            // will fail.
            if (actualStart == _start.CompareExchange(newStart, actualStart))
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
            var actualEnd = _end.LoadInterlocked();
            // check if pool is not locked (no operation is in progress)
            if (!actualEnd.Locked)
            {
                var actualStart = _start; // NOTE: COPY
                if (actualStart == actualEnd)
                {
                    // recheck
                    if (!(actualEnd == _end.LoadInterlocked() && actualStart == _start.LoadInterlocked()))
                    {
                        continue;
                    }
                    // no items avaliable
                    item = default;
                    return false;
                }
                // preload item --> it will be returned if the operation succeeds
                var actualStartValue = actualStart.Value;
                var candidate = UnsafeItem(actualStartValue); // NOTE: force no bound checks
                var newStart = actualStartValue == _maxIndex
                    ? actualStart.ToggleLoop()
                    : actualStart.Inc();
                if (actualStart == _start.CompareExchange(newStart, actualStart))
                {
                    // operation has succeeded
#if DEBUG
                    Debug.Assert(candidate is not null, "instance is null");
#endif
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
        var actualEnd = _end; // NOTE: copy, out-of-order acccess
        while (true)
        {
            // check if pool is not locked (no operation is in progress)
            if (!actualEnd.Locked)
            {
                var actualStart = _start.LoadInterlocked();
                var capacity = _items.Length;
                var currentSize = Ixx.ComputeSize(actualStart, actualEnd, unchecked((uint)capacity));
                // if computing size result is negative _end must have changed (it was load via relaxed access)
                // thus it must be reloaded via interlocked access and the operation must be retried
                if (currentSize >= 0)
                {
                    if (capacity == currentSize)
                    {
                        // recheck whether start/end has changed
                        if (actualEnd == _end.LoadInterlocked() && actualStart == _start.LoadInterlocked())
                        {
                            // if sync is ok allow GC to claim an item (default impl) or perform overridden cleanup
                            Cleanup(item);
                            return;
                        }
                    }
                    else
                    {
                        var newEnd = actualEnd.Value == _maxIndex
                            ? actualEnd.ToggleLoop()
                            : actualEnd.Inc();
                        // Two step value application:
                        // If first step succeedes --> pool is in locked state and the item can be stored safely
                        // second step --> pool is unlocked
                        if (_end.TryLock(actualEnd, newEnd, out var maskedEnd))
                        {
                            // pool is locked --> proceed to store value and unlock pool
                            UnsafeItem(actualEnd.Value, item); // _items[Ix.GetValueInline(actualEnd)] = item;
                            var xres = _end.CompareExchange(newEnd, maskedEnd); // should always succeed
#if DEBUG
                            Debug.Assert(xres.Equals(maskedEnd), "Unlock end failed!");
#endif
                            return;
                        }
                        // (else is noop) update has failed --> retry operation
                    }
                }
            }
            // (else is noop) pool is locked (operation is in progress) --> retry current operation
            actualEnd = _end.LoadInterlocked();
        }
    }
}
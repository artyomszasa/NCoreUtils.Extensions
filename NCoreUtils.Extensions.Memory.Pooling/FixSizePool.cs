using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using NCoreUtils.Memory.Pooling;

namespace NCoreUtils;

internal struct Index : IEquatable<Index>
{
    private const uint MaskValue = 0x0000FFFF;

    private const uint MaskLocked = 0x80000000;

    private const uint MaskLoop = 0x40000000;

    [MethodImpl(O.Inline)]
    [DebuggerStepThrough]
    public static bool operator==(Index a, Index b)
        => a.Equals(b);

    [MethodImpl(O.Inline)]
    [DebuggerStepThrough]
    public static bool operator!=(Index a, Index b)
        => !a.Equals(b);

    private uint _data;

    [MethodImpl(O.Inline)]
    [DebuggerStepThrough]
    public Index(uint data)
        => _data = data;

    public readonly uint Value
    {
        [MethodImpl(O.Inline)]
        [DebuggerStepThrough]
        get => _data & MaskValue;
    }

    public readonly bool Loop
    {
        [MethodImpl(O.Inline)]
        [DebuggerStepThrough]
        get => (_data & MaskLoop) != 0;
    }

    public readonly bool Locked
    {
        [MethodImpl(O.Inline)]
        [DebuggerStepThrough]
        get => (_data & MaskLocked) != 0;
    }

    [MethodImpl(O.Inline)]
    [DebuggerStepThrough]
    public readonly Index Inc()
        => new(_data + 1);

    [MethodImpl(O.Inline)]
    [DebuggerStepThrough]
    public readonly Index ToggleLoop()
        => new((_data & (~MaskValue)) ^ MaskLoop);

    [MethodImpl(O.Inline)]
    [DebuggerStepThrough]
    public readonly Index Lock()
        => new(_data | MaskLocked);

    [MethodImpl(O.Inline)]
    [DebuggerStepThrough]
    public Index CompareExchange(Index value, Index comparand)
#if NET6_0_OR_GREATER
        => new(Interlocked.CompareExchange(ref _data, value._data, comparand._data));
#else
    {
        var ival = Interlocked.CompareExchange(ref Unsafe.As<uint, int>(ref _data), unchecked((int)value._data), unchecked((int)comparand._data));
        return new(unchecked((uint)ival));
    }
#endif

    [MethodImpl(O.Inline)]
    [DebuggerStepThrough]
    public Index Load()
#if NET6_0_OR_GREATER
        => new(Interlocked.CompareExchange(ref _data, default, default));
#else
    {
        var ival = Interlocked.CompareExchange(ref Unsafe.As<uint, int>(ref _data), unchecked((int)default), unchecked((int)default));
        return new(unchecked((uint)ival));
    }
#endif

    [MethodImpl(O.Inline)]
    [DebuggerStepThrough]
    public readonly bool Equals(Index other)
        => _data == other._data;

    [MethodImpl(O.Inline)]
    [DebuggerStepThrough]
    public readonly bool EqLoop(Index other)
        => (_data & (MaskLoop | MaskValue)) == (other._data & (MaskLoop | MaskValue));

    public readonly override bool Equals([NotNullWhen(true)] object? obj)
        => obj is Index other && Equals(other);

    public readonly override int GetHashCode()
        => unchecked((int)_data);

    [ExcludeFromCodeCoverage]
    public readonly override string ToString()
        => $"{Value} [Loop = {Loop}, Locked = {Locked}]";
}

internal static class FixSizePoolHelpers
{
    [MethodImpl(O.Optimize)]
    internal static int ComputeSize(Index start, Index end, int capacity)
    {
        if (start.Loop == end.Loop)
        {
            return unchecked((int)(end.Value - start.Value));
        }
        return unchecked((int)end.Value) + capacity - unchecked((int)start.Value);
    }
}

public class FixSizePool<T> : IObjectPool<T>
    where T : class
{

    private readonly T?[] _items;

    private readonly uint _maxIndex;

    private Index _start;

    private Index _end;

    public int AvailableCount
    {
        [MethodImpl(O.Inline)]
        get => FixSizePoolHelpers.ComputeSize(
            start: _start.Load(),
            end: _end.Load(),
            capacity: _items.Length
        );
    }

    public FixSizePool(int size)
    {
        if (size < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }
        _items = new T[size];
        _maxIndex = unchecked((uint)size - 1);
    }

    // NOTE: UNIT TEST ONLY
    internal void UnsafeReset()
    {
        for (var i = 0; i < _items.Length; ++i)
        {
            _items[i] = default!;
        }
    }

    [MethodImpl(O.Optimize)]
    private bool TryRentShort([MaybeNullWhen(false)] out T item)
    {
        var actualStart = _start; // relaxed
        var actualEnd = _end; // relaxed
        // NOTE: both indices are monotonically increasing --> if they are not equal then start is lower than end.
        if (!actualStart.EqLoop(actualEnd))
        {
            var sv = actualStart.Value;
            var candidate = _items[sv];
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

    private bool TryRentLong([MaybeNullWhen(false)] out T item)
    {
        while (true)
        {
            var actualEnd = _end.Load();
            // check if pool is not locked (no operation is in progress)
            if (!actualEnd.Locked)
            {
                var actualStart = _start.Load();
                if (actualStart == actualEnd)
                {
                    // no items avaliable
                    item = default;
                    return false;
                }
                // preload item --> it will be returned if the operation succeeds
                var candidate = _items[actualStart.Value];
                Index newStart;
                if (actualStart.Value == _maxIndex)
                {
                    newStart = actualStart.ToggleLoop();
                }
                else
                {
                    newStart = actualStart.Inc();
                }
                if (actualStart == _start.CompareExchange(newStart, actualStart))
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

    [MethodImpl(O.Inline)]
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
            var actualStart = _start.Load();
            var actualEnd = _end.Load();
            // check if pool is not locked (no operation is in progress)
            if (!actualEnd.Locked)
            {
                if (_items.Length == FixSizePoolHelpers.ComputeSize(actualStart, actualEnd, _items.Length))
                {
                    // recheck whether start/end has changed
                    if (actualEnd != _end.Load() || actualStart != _start.Load())
                    {
                        // if so --> retry
                        success = false;
                        continue;
                    }
                    // otherwise allow GC to claim an item (default impl) or perform overridden cleanup
                    Cleanup(item);
                    return;
                }
                Index newEnd;
                if (actualEnd.Value == _maxIndex)
                {
                    newEnd = actualEnd.ToggleLoop();
                }
                else
                {
                    newEnd = actualEnd.Inc();
                }
                // Two step value application:
                // If first step succeedes --> pool is in locked state and the item can be stored safely
                // second step --> pool is unlocked
                var maskedEnd = newEnd.Lock();
                if (actualEnd == _end.CompareExchange(maskedEnd, actualEnd))
                {
                    // pool is locked --> proceed to store value and unlock pool
                    _items[actualEnd.Value] = item;
                    _end.CompareExchange(newEnd, maskedEnd); // should always succeed
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

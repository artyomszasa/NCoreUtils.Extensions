using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using NCoreUtils.Memory.Pooling;

namespace NCoreUtils;

static class IxLe
{
    // layout: |VAL-|LOOP|--LOCK--|
    // 4bit    |0--3|4--7|8------F|
    // byte    |0--1|2--3|4------7|
    // ushort  |0---|1---|2------3|
    // uint    |0--------|1-------|

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Compose(ushort value, bool loop, bool locked)
    {
        ulong data = value | (loop ? 1u << 16 : 0ul) | (locked ? 1ul << 32 : 0ul);
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref ushort GetValueRef(ref Ix index)
        => ref Unsafe.As<ulong, ushort>(ref Unsafe.AsRef(in index._data));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref ushort GetLoopRef(ref Ix index)
        => ref Unsafe.Add(ref Unsafe.As<ulong, ushort>(ref Unsafe.AsRef(in index._data)), 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref uint GetLoopValueRef(ref Ix index)
        => ref Unsafe.As<ulong, uint>(ref Unsafe.AsRef(in index._data));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref uint GetLockRef(ref Ix index)
        => ref Unsafe.Add(ref Unsafe.As<ulong, uint>(ref Unsafe.AsRef(in index._data)), 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLocked(Ix index)
    {
        return (0xFFFFFFFF00000000UL & index._data) != 0;
    }
}

static class IxBe
{
    // layout: |--LOCK--|LOOP|VAL-|
    // 4bit    |0------7|8--B|C--F|
    // byte    |0------3|4--5|6--7|
    // ushort  |0------1|2---|3---|
    // uint    |0-------|1--------|

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Compose(ushort value, bool loop, bool locked)
    {
        ulong data = value | (loop ? 1u << 24 : 0ul) | (locked ? 1ul << 56 : 0ul);
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref ushort GetValueRef(ref Ix index)
        => ref Unsafe.Add(ref Unsafe.As<ulong, ushort>(ref Unsafe.AsRef(in index._data)), 3);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref ushort GetLoopRef(ref Ix index)
        => ref Unsafe.Add(ref Unsafe.As<ulong, ushort>(ref Unsafe.AsRef(in index._data)), 2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref uint GetLoopValueRef(ref Ix index)
        => ref Unsafe.Add(ref Unsafe.As<ulong, uint>(ref Unsafe.AsRef(in index._data)), 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref uint GetLockRef(ref Ix index)
        => ref Unsafe.As<ulong, uint>(ref Unsafe.AsRef(in index._data));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLocked(Ix index)
    {
        return (0xFFFFFFFF00000000UL & index._data) != 0;
    }
}

#if DEBUG
[DebuggerDisplay("{DebugDisplayString}")]
#endif
[DebuggerTypeProxy(typeof(IxDebugView))]
readonly struct Ix : IEquatable<Ix>
{
    // private const uint MaskValue = 0x000FFFFu;

    // private const uint MaskLocked = 0x80000000u;

    // private const uint MaskLoop = 0x00008000u;

    internal class IxDebugView(Ix value)
    {
        private readonly Ix _value = value;

        public int Value => (int)GetValue(in _value);

        public bool Loop => (_value._data & 0x0000000000010000UL) != 0;

        public bool Locked => IsLocked(in _value);
    }

#if DEBUG

    public string DebugDisplayString
    {
        get
        {
            var value = GetValue(in this);
            var loop = (_data & 0x0000000000010000UL) != 0;
            var locked = IsLocked(in this);
            if (loop)
            {
                if (locked)
                {
                    return $"{value} LOOP LOCKED";
                }
                return $"{value} LOOP";
            }
            if (locked)
            {
                return $"{value} LOCKED";
            }
            return value.ToString();
        }
    }

#endif

    #region polyfill

#if NET6_0_OR_GREATER

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong UInt64CompareExchange(ref ulong location, ulong value, ulong comparand)
        => Interlocked.CompareExchange(ref location, value, comparand);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong UInt64InterlockedRead(ref ulong location)
        => Interlocked.Read(ref location);

#else

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong UInt64CompareExchange(ref ulong location, ulong value, ulong comparand)
    {
        var ival = Interlocked.CompareExchange(ref Unsafe.As<ulong, long>(ref location), unchecked((long)value), unchecked((long)comparand));
        return unchecked((ulong)ival);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong UInt64InterlockedRead(ref ulong location)
        => unchecked((ulong)Interlocked.Read(ref Unsafe.As<ulong, long>(ref location)));

#endif

    #endregion

    #region equality

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Ix other)
        => _data == other._data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool InterlockedEquals(in Ix other)
        => _data == UInt64InterlockedRead(ref Unsafe.AsRef(in other._data));

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is Ix other && Equals(other);

    public override int GetHashCode()
        => _data.GetHashCode();

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetValue(in Ix index)
    {
        if (BitConverter.IsLittleEndian)
        {
            return IxLe.GetValueRef(ref Unsafe.AsRef(in index));
        }
        return IxBe.GetValueRef(ref Unsafe.AsRef(in index));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetValueInline(Ix index)
    {
        return unchecked((uint)(index._data & 0x0FFFFul));
    }

    public static bool IsLockedInline(Ix index)
    {
        if (BitConverter.IsLittleEndian)
        {
            return IxLe.IsLocked(index);
        }
        return IxBe.IsLocked(index);
    }

    public static bool IsLocked(in Ix index)
    {
        if (BitConverter.IsLittleEndian)
        {
            return 0 != IxLe.GetLockRef(ref Unsafe.AsRef(in index));
        }
        return 0 != IxBe.GetLockRef(ref Unsafe.AsRef(in index));
    }

    internal readonly ulong _data;

    [MethodImpl(O.Optimize | O.Inline)]
    [DebuggerStepThrough]
    public static bool EqLoop(in Ix index0, in Ix index1)
    {
        if (BitConverter.IsLittleEndian)
        {
            return IxLe.GetLoopValueRef(ref Unsafe.AsRef(in index0)) == IxLe.GetLoopValueRef(ref Unsafe.AsRef(in index1));
        }
        return IxBe.GetLoopValueRef(ref Unsafe.AsRef(in index0)) == IxBe.GetLoopValueRef(ref Unsafe.AsRef(in index1));
    }

    [MethodImpl(O.Optimize | O.Inline)]
    [DebuggerStepThrough]
    public static bool EqLoopInline(Ix index0, in Ix index1)
    {
        if (BitConverter.IsLittleEndian)
        {
            return GetValueInline(index0) == IxLe.GetLoopValueRef(ref Unsafe.AsRef(in index1));
        }
        return GetValueInline(index0) == IxBe.GetLoopValueRef(ref Unsafe.AsRef(in index1));
    }

    [MethodImpl(O.Inline | O.Optimize)]
    public static Ix CompareExchange(ref Ix location, Ix value, Ix comparand)
    {
        var res = UInt64CompareExchange(ref Unsafe.AsRef(in location._data), value._data, comparand._data);
        return new(res);
    }

    [MethodImpl(O.Inline)]
    public static Ix Load(in Ix source)
        => new(Volatile.Read(ref Unsafe.AsRef(in source._data)));

    [MethodImpl(O.Inline)]
    public static Ix Inc(in Ix source)
        => new(source._data + 1);

    [MethodImpl(O.Inline)]
    public static Ix IncInline(Ix source)
        => new(source._data + 1);

    [MethodImpl(O.Inline | O.Optimize)]
    public static bool TryLock(ref Ix target, Ix comparand, Ix value, out Ix newValue)
    {
        newValue = value; // NOT: copy
        if (BitConverter.IsLittleEndian)
        {
            IxLe.GetLockRef(ref newValue) = 1;
        }
        else
        {
            IxBe.GetLockRef(ref newValue) = 1;
        }
        return comparand.Equals(CompareExchange(ref target, newValue, comparand));
    }

    public static Ix ToggleLoopInline(Ix value)
    {
        return new Ix((value._data & 0x0000000000010000UL) ^ 0x0000000000010000UL);
    }

    [MethodImpl(O.Inline | O.Optimize)]
    public static int ComputeSizeInline(Ix start, Ix end, uint capacity)
    {
        // IsLoop(start) == IsLoop(end)
        if (((start._data ^ end._data) & 0x0000000000010000UL) == 0)
        {
            var size = unchecked((int)(GetValueInline(end) - GetValueInline(start)));
            Debug.Assert(size >= 0, "computed size is less than zero");
            return size;
        }
        var sizeLooped = unchecked((int)(GetValueInline(end) + capacity - GetValueInline(start)));
        Debug.Assert(sizeLooped >= 0, "computed size (looped) is less than zero");
        return sizeLooped;
    }

    [MethodImpl(O.Optimize)]
    public static int ComputeSize(in Ix start, in Ix end, uint capacity)
    {
        if (BitConverter.IsLittleEndian)
        {
            if (IxLe.GetLoopRef(ref Unsafe.AsRef(in start)) == IxLe.GetLoopRef(ref Unsafe.AsRef(in end)))
            {
                var size = unchecked((int)(GetValue(in end) - GetValue(in start)));
                Debug.Assert(size >= 0, "computed size is less than zero");
                return size;
            }
            var sizeLooped0 = unchecked((int)(IxLe.GetValueRef(ref Unsafe.AsRef(in end)) + capacity - IxLe.GetValueRef(ref Unsafe.AsRef(in start))));
            Debug.Assert(sizeLooped0 >= 0, "computed size (looped) is less than zero");
            return sizeLooped0;
        }
        if (IxBe.GetLoopRef(ref Unsafe.AsRef(in start)) == IxBe.GetLoopRef(ref Unsafe.AsRef(in end)))
        {
            var size = unchecked((int)(GetValue(in end) - GetValue(in start)));
            Debug.Assert(size >= 0, "computed size is less than zero");
            return size;
        }
        var sizeLooped = unchecked((int)(IxBe.GetValueRef(ref Unsafe.AsRef(in end)) + capacity - IxBe.GetValueRef(ref Unsafe.AsRef(in start))));
        Debug.Assert(sizeLooped >= 0, "computed size (looped) is less than zero");
        return sizeLooped;
    }

    // [FieldOffset(0)]
    // private readonly ushort _value;
    // [FieldOffset(sizeof(ushort))]
    // private readonly uint _lock;


    // public uint Value
    // {
    //     get
    //     {
    //         if (BitConverter.IsLittleEndian)
    //         {
    //             IxLe.GetValue(in this);
    //         }
    //         return IxBe.GetValue(in this);
    //     }
    // }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Ix(ulong data) => _data = data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Ix(ushort value, bool loop, bool locked)
    {
        if (BitConverter.IsLittleEndian)
        {
            _data = IxLe.Compose(value, loop, locked);
        }
        else
        {
            _data = IxBe.Compose(value, loop, locked);
        }
    }
}
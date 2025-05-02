using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using NCoreUtils.Memory.Pooling;

namespace NCoreUtils;

#if DEBUG
[DebuggerDisplay("{DebugDisplayString}")]
#endif
[DebuggerTypeProxy(typeof(IxiDebugView))]
public readonly struct Ixi : IEquatable<Ixi>
{
    #region debug

    internal class IxiDebugView(Ixi value)
    {
        public uint Value => value.Value;

        public bool Loop => value.Loop;

        public bool Locked => value.Locked;
    }

#if DEBUG

    public string DebugDisplayString
    {
        get
        {
            var value = Value;
            var loop = Loop;
            var locked = Locked;
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

    #endregion

    internal const uint MaskValue = 0x0000FFFFu;

    internal const uint MaskLocked = 0x80000000u;

    internal const uint MaskLoop = 0x40000000u;

    internal readonly uint _data;

    #region construction

    [MethodImpl(O.Inline)]
    internal Ixi(uint data) => _data = data;

    [MethodImpl(O.Inline)]
    public Ixi(ushort value, bool loop, bool locked)
        : this(value | (loop ? MaskLoop : 0u) | (locked ? MaskLocked : 0u)) // FIXME: optimize
    { }

    #endregion

    #region access

    public uint Value
    {
        [MethodImpl(O.Inline)]
        [DebuggerStepThrough]
        get => _data & MaskValue;
    }

    public bool Loop
    {
        [MethodImpl(O.Inline)]
        [DebuggerStepThrough]
        get => (_data & MaskLoop) != 0;
    }

    public bool Locked
    {
        [MethodImpl(O.Inline)]
        [DebuggerStepThrough]
        get => (_data & MaskLocked) != 0;
    }

    #endregion

    #region mutation

    [MethodImpl(O.Inline)]
    [DebuggerStepThrough]
    public Ixi Inc()
        => new(_data + 1);

    [MethodImpl(O.Inline)]
    [DebuggerStepThrough]
    public Ixi ToggleLoop()
        => new((_data & MaskLoop) ^ MaskLoop);

    [MethodImpl(O.Inline)]
    [DebuggerStepThrough]
    public Ixi Lock()
        => new(_data | MaskLocked);

    #endregion

    #region equality

    public static bool operator== (Ixi a, Ixi b) => a.Equals(b);

    public static bool operator!= (Ixi a, Ixi b) => !a.Equals(b);

    public bool Equals(Ixi other)
        => _data == other._data;

    public bool Eq(Ixi other)
        => (_data & ~MaskLocked) == (other._data & ~MaskLocked);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is Ixi other && Equals(other);

    public override int GetHashCode()
        => unchecked((int)_data);

    #endregion
}

public static class Ixx
{
    [MethodImpl(O.Inline)]
    public static Ixi CompareExchange(this ref Ixi location, Ixi value, Ixi comparand)
        => unchecked(new((uint)Interlocked.CompareExchange(
            ref Unsafe.As<Ixi, int>(ref location),
            (int)value._data,
            (int)comparand._data
        )));

    /// <summary>
    /// Relaxed load of the value.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    [MethodImpl(O.Inline)]
    public static Ixi LoadRelaxed(this in Ixi location)
        => new(Volatile.Read(ref Unsafe.As<Ixi, uint>(ref Unsafe.AsRef(in location))));

    /// <summary>
    /// Interlocked load of the value.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    [MethodImpl(O.Inline)]
    public static Ixi LoadInterlocked(this in Ixi location)
        => CompareExchange(ref Unsafe.AsRef(in location), default, default);

    [MethodImpl(O.Inline | O.Optimize)]
    public static bool TryLock(this ref Ixi target, Ixi comparand, Ixi value, out Ixi newValue)
    {
#if DEBUG
        Debug.Assert(!value.Locked, "value is already locked");
#endif
        newValue = value.Lock();
        return comparand == CompareExchange(
            ref target,
            newValue,
            comparand
        );
    }

    [MethodImpl(O.Inline | O.Optimize)]
    public static int ComputeSize(Ixi start, Ixi end, uint capacity)
    {
        // IsLoop(start) == IsLoop(end)
        if (((start._data ^ end._data) & Ixi.MaskLoop) == 0)
        {
            return unchecked((int)(end.Value - start.Value));
        }
        return unchecked((int)(end.Value + capacity - start.Value));
    }
}
using System.Runtime.CompilerServices;
using System.Threading;

namespace NCoreUtils;

public struct InterlockedBoolean
{
    private int _value;

    public bool Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 0 != Volatile.Read(ref _value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TrySet()
        => 0 == Interlocked.CompareExchange(ref _value, 1, 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReset()
        => 1 == Interlocked.CompareExchange(ref _value, 0, 1);
}
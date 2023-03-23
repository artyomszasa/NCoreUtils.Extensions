using System;

namespace NCoreUtils;

public sealed class DisposingFixSizePool<T> : FixSizePool<T>
    where T : class, IDisposable
{
    public DisposingFixSizePool(int size) : base(size) { }

    protected override void Cleanup(T item)
        => item.Dispose();
}
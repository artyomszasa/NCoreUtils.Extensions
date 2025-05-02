namespace NCoreUtils;

public sealed class DisposingFixSizePool<T>(int size)
    : FixSizePool<T>(size)
    where T : class, IDisposable
{
    protected override void Cleanup(T item)
        => item.Dispose();
}
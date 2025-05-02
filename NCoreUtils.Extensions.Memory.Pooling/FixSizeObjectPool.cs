using Microsoft.Extensions.ObjectPool;

namespace NCoreUtils;

public class FixSizeObjectPool<T>(int size, IPooledObjectPolicy<T> policy)
    : ObjectPool<T>
    where T : class
{
    private readonly IPooledObjectPolicy<T> _policy = policy ?? throw new ArgumentNullException(nameof(policy));

    private readonly FixSizePool<T> _pool = new(size);

    public override T Get()
        => _pool.TryRent(out var instance) ? instance : _policy.Create();

    public override void Return(T obj)
    {
        if (_policy.Return(obj))
        {
            _pool.Return(obj);
        }
    }
}
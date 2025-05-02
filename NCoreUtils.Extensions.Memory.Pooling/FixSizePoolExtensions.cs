using System.Runtime.CompilerServices;

namespace NCoreUtils;

public interface IReusePolicy<T, T1>
{
    T Create(T1 arg1);

    T Update(T instance, T1 arg1);
}

public interface IReusePolicy<T, T1, T2>
{
    T Create(T1 arg1, T2 arg2);

    T Update(T instance, T1 arg1, T2 arg2);
}

public interface IReusePolicy<T, T1, T2, T3>
{
    T Create(T1 arg1, T2 arg2, T3 arg3);

    T Update(T instance, T1 arg1, T2 arg2, T3 arg3);
}

public static class FixSizePoolExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReuseOrCreate<T, T1>(this FixSizePool<T> pool, IReusePolicy<T, T1> policy, T1 arg)
        where T : class
        => pool.TryRent(out var instance)
            ? policy.Update(instance, arg)
            : policy.Create(arg);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReuseOrCreate<T, T1, T2>(this FixSizePool<T> pool, IReusePolicy<T, T1, T2> policy, T1 arg1, T2 arg2)
        where T : class
        => pool.TryRent(out var instance)
            ? policy.Update(instance, arg1, arg2)
            : policy.Create(arg1, arg2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReuseOrCreate<T, T1, T2, T3>(this FixSizePool<T> pool, IReusePolicy<T, T1, T2, T3> policy, T1 arg1, T2 arg2, T3 arg3)
        where T : class
        => pool.TryRent(out var instance)
            ? policy.Update(instance, arg1, arg2, arg3)
            : policy.Create(arg1, arg2, arg3);
}
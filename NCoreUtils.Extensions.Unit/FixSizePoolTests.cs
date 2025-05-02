using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ThePool = NCoreUtils.FixSizePool<NCoreUtils.Extensions.Unit.FixSizePoolTests.Obj>;

namespace NCoreUtils.Extensions.Unit;

public class FixSizePoolTests
{
    internal sealed class Counter
    {
        public int Ctor;

        public int Dtor;
    }

    internal sealed class Obj
    {
        private readonly Counter _counter;

        public Obj(Counter counter)
        {
            _counter = counter;
            Interlocked.Increment(ref _counter.Ctor);
        }

        ~Obj()
        {
            Interlocked.Increment(ref _counter.Dtor);
        }
    }

    private static void ForceGC()
    {
        GC.Collect();
        GC.Collect(0, GCCollectionMode.Forced, blocking: true);
        GC.WaitForFullGCComplete();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        GC.Collect(0, GCCollectionMode.Forced, blocking: true);
        GC.WaitForFullGCComplete();
        GC.WaitForPendingFinalizers();
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private static bool TryRentIgnore(ThePool pool)
        => pool.TryRent(out var _);

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private static void RentAndReturn(ThePool pool)
    {
        Assert.True(pool.TryRent(out var obj));
        Thread.Yield();
        pool.Return(obj!);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void RunTasks(ThePool pool, Counter counter, SemaphoreSlim trigger)
    {
        var tasks = new int[16].Select(_ => Task.Factory.StartNew(() =>
        {
            trigger.Wait();
            try
            {
                for (var i = 0; i < 2048; ++i)
                {
                    RentAndReturn(pool);
                }
            }
            finally
            {
                trigger.Release();
            }
        }, TaskCreationOptions.None)).ToArray();
        for (var i = 0; i < 16; ++i)
        {
            Insert(pool, counter);
        }
        trigger.Release(16);
        Task.WaitAll(tasks);
        foreach (var task in tasks)
        {
            if (task.IsFaulted)
            {
                ExceptionDispatchInfo.Capture(task.Exception).Throw();
            }
            task.Dispose();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Insert(ThePool pool, Counter counter)
        => pool.Return(new Obj(counter));

    [Fact]
    public void One()
    {
        var pool = new ThePool(16);
        Assert.False(pool.TryRent(out var __));
        Assert.Equal("item", Assert.Throws<ArgumentNullException>(() => pool.Return(default!)).ParamName);
        var counter = new Counter();
        for (var i = 0; i < 16; ++i)
        {
            Insert(pool, counter);
        }
        for (var i = 0; i < 8; ++i)
        {
            TryRentIgnore(pool);
        }
        for (var i = 0; i < 8; ++i)
        {
            Insert(pool, counter);
        }
        for (var i = 0; i < 16; ++i)
        {
            TryRentIgnore(pool);
        }
        pool.UnsafeReset();
        GC.Collect(0, GCCollectionMode.Forced, blocking: true);
        GC.WaitForFullGCComplete();
        GC.WaitForPendingFinalizers();
        Assert.Equal(24, counter.Ctor);
        Assert.Equal(24, counter.Dtor);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void RunBasic(Counter counter)
    {
        var pool = new ThePool(16);
        using var trigger = new SemaphoreSlim(0, 16);
        RunTasks(pool, counter, trigger);
        ForceGC();
        Assert.Equal(16, pool.AvailableCount);
        Assert.Equal(16, counter.Ctor);
        Assert.Equal(0, counter.Dtor);
        for (var i = 0; i < 16; ++i)
        {
            if (!TryRentIgnore(pool))
            {
                Assert.True(false);
            }
        }
        Assert.Equal(0, pool.AvailableCount);
        pool.UnsafeReset();
        ForceGC();
    }

    [Fact]
    public void Basic()
    {
        var counter = new Counter();
        RunBasic(counter);
        Thread.SpinWait(128);
        ForceGC();
        Assert.Equal(16, counter.Ctor);
        Assert.Equal(16, counter.Dtor);
    }

    [Fact]
    public void Invalid()
    {
        Assert.Equal("size", Assert.Throws<ArgumentOutOfRangeException>(() => new ThePool(-20)).ParamName);
        var pool = new ThePool(4);
        var counter = new Counter();
        AddFiveObjects(pool, counter);
        ForceGC();
        Assert.Equal(5, counter.Ctor);
        Assert.Equal(1, counter.Dtor);

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        static void AddFiveObjects(ThePool pool, Counter counter)
        {
            pool.Return(new(counter));
            pool.Return(new(counter));
            pool.Return(new(counter));
            pool.Return(new(counter));
            pool.Return(new(counter));
        }
    }
}
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NCoreUtils.Extensions.Unit;

public class FixSizePoolTests
{
    private sealed class Counter
    {
        public int Ctor;

        public int Dtor;
    }

    private sealed class Obj
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
    private static bool TryRentIgnore(FixSizePool<Obj> pool)
        => pool.TryRent(out var _);

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private static void RentAndReturn(FixSizePool<Obj> pool)
    {
        Assert.True(pool.TryRent(out var obj));
        Thread.Yield();
        pool.Return(obj!);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void RunTasks(FixSizePool<Obj> pool, Counter counter, SemaphoreSlim trigger)
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
            task.Dispose();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Insert(FixSizePool<Obj> pool, Counter counter)
        => pool.Return(new Obj(counter));

    [Fact]
    public void One()
    {
        var pool = new FixSizePool<Obj>(16);
        Assert.False(pool.TryRent(out var __));
        Assert.Equal("item", Assert.Throws<ArgumentNullException>(() => pool.Return(default!)).ParamName);
        var counter = new Counter();
        for (var i = 0; i < 16; ++i)
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
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void RunBasic(Counter counter)
    {
        var pool = new FixSizePool<Obj>(16);
        using var trigger = new SemaphoreSlim(0, 16);
        RunTasks(pool, counter, trigger);
        ForceGC();
        Assert.Equal(16, pool.AvailableCount);
        Assert.Equal(16, counter.Ctor);
        Assert.Equal(0, counter.Dtor);
        for (var i = 0; i < 16; ++i)
        {
            Assert.True(TryRentIgnore(pool));
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
        Assert.Equal("size", Assert.Throws<ArgumentOutOfRangeException>(() => new FixSizePool<Obj>(-20)).ParamName);
        var pool = new FixSizePool<Obj>(4);
        var counter = new Counter();
        AddFiveObjects(pool, counter);
        ForceGC();
        Assert.Equal(5, counter.Ctor);
        Assert.Equal(1, counter.Dtor);

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        static void AddFiveObjects(FixSizePool<Obj> pool, Counter counter)
        {
            pool.Return(new(counter));
            pool.Return(new(counter));
            pool.Return(new(counter));
            pool.Return(new(counter));
            pool.Return(new(counter));
        }
    }

    [Fact]
    public void IndexMisc()
    {
        var index = new Index(32);
        Assert.Equal(32, index.GetHashCode());
        Assert.True(((object)index).Equals(new Index(32)));
        Assert.False(((object)index).Equals(32));
    }
}
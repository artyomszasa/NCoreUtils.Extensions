using System;
using System.Linq;
using Xunit;

namespace NCoreUtils
{
    public class UnmanagedMemoryTests
    {
        [Fact]
        public void InvalidArguments()
        {
            // unaligned size
            Assert.Throws<ArgumentException>(() => new UnmanagedMemoryPool<byte>(4 * 1024 + 5, 4 * 1024, 4 * 1024));
            Assert.Throws<ArgumentException>(() => new UnmanagedMemoryPool<byte>(4 * 1024, 4 * 1024 + 5, 4 * 1024));
            Assert.Throws<ArgumentException>(() => new UnmanagedMemoryPool<byte>(4 * 1024, 4 * 1024, 4 * 1024 + 5));

            // unaligned size (not pow2)
            Assert.Throws<ArgumentException>(() => new UnmanagedMemoryPool<byte>(3 * 1024, 4 * 1024, 4 * 1024));
            Assert.Throws<ArgumentException>(() => new UnmanagedMemoryPool<byte>(4 * 1024, 3 * 1024, 4 * 1024));
            Assert.Throws<ArgumentException>(() => new UnmanagedMemoryPool<byte>(4 * 1024, 4 * 1024, 3 * 1024));

            // minBuffer > maxBuffer
            Assert.Throws<InvalidOperationException>(() => new UnmanagedMemoryPool<byte>(4 * 1024, 2 * 1024, 2 * 1024));

            // defaultBuffer > maxBuffer
            Assert.Throws<InvalidOperationException>(() => new UnmanagedMemoryPool<byte>(2 * 1024, 4 * 1024, 8 * 1024));

            // defaultBuffer < minBuffer
            Assert.Throws<InvalidOperationException>(() => new UnmanagedMemoryPool<byte>(4 * 1024, 8 * 1024, 2 * 1024));
        }

        [Fact]
        public void DisposeTests()
        {
            using var pool = new UnmanagedMemoryPool<byte>();
            Assert.Equal(UnmanagedMemoryPool<byte>.DefaultDefaultBufferSize, pool.DefaultBufferSize);
            Assert.Equal(UnmanagedMemoryPool<byte>.DefaultMaxBufferSize, pool.MaxBufferSize);
            var store = pool.Store;
            {
                var owner = pool.Rent(4100);
                Assert.Equal(8 * 1024, owner.Memory.Length);
                Assert.True(store.All(item => item.Queue.IsEmpty));
                owner.Dispose();
                Assert.Single(store, item => item.MaxSize == 8 * 1024 && item.Queue.Count == 1);
            }
            Assert.Single(store, item => item.MaxSize == 8 * 1024 && item.Queue.Count == 1);
            pool.Dispose();
            Assert.Throws<ObjectDisposedException>(() => pool.Rent());
            Assert.True(store.All(item => item.Queue.IsEmpty));
        }
    }
}
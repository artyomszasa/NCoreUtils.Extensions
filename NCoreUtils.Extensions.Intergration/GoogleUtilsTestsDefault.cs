using System;
using System.Buffers;
using System.Threading.Tasks;
using Xunit;

namespace NCoreUtils
{
    public class GoogleUtilsTestsDefault : GoogleUnitTestsBase
    {
        private class DummyMemoryOwner : IMemoryOwner<byte>
        {
            private bool _isDisposed;

            private byte[] _data;

            public bool IsDisposed => _isDisposed;

            public DummyMemoryOwner(int size)
                => _data = new byte[size];

            public Memory<byte> Memory => _data.AsMemory();

            public void Dispose()
            {
                _isDisposed = true;
            }
        }

        public GoogleUtilsTestsDefault()
            : base(default)
        { }

        [Fact]
        public Task CreateGetCopyDeleteSharedBuffer()
        {
            return base.CreateGetCopyDelete(default);
        }

        [Fact]
        public async Task CreateGetCopyDeleteUnmanagedBuffer()
        {
            using var pool = new UnmanagedMemoryPool<byte>();
            await base.CreateGetCopyDelete(pool.Rent(512 * 1024));
        }

        [Fact]
        public async Task CreateGetCopyDeleteUnalignedBuffer()
        {
            var buffer = new DummyMemoryOwner(512 * 1024 + 512);
            await base.CreateGetCopyDelete(buffer);
            Assert.True(buffer.IsDisposed);
        }
    }
}
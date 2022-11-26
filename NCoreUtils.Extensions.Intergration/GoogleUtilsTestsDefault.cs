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

            private readonly byte[] _data;

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

        [Theory]
        [InlineData("tmp.jpg")]
        [InlineData("тмп.jpg")]
        public Task CreateGetCopyDeleteSharedBuffer(string objectName)
        {
            return base.CreateGetCopyDelete(default, objectName);
        }

        [Theory]
        [InlineData("tmpum.jpg")]
        [InlineData("тмпum.jpg")]
        public async Task CreateGetCopyDeleteUnmanagedBuffer(string objectName)
        {
            using var pool = new UnmanagedMemoryPool<byte>();
            await base.CreateGetCopyDelete(pool.Rent(512 * 1024), objectName);
        }

        [Theory]
        [InlineData("tmpua.jpg")]
        [InlineData("тмпua.jpg")]
        public async Task CreateGetCopyDeleteUnalignedBuffer(string objectName)
        {
            var buffer = new DummyMemoryOwner(512 * 1024 + 512);
            await base.CreateGetCopyDelete(buffer, objectName);
            Assert.True(buffer.IsDisposed);
        }
    }
}
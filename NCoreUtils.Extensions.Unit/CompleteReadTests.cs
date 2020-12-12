using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NCoreUtils
{
    public class CompleteReadTests
    {
        private sealed class DelayedStream : Stream
        {
            private readonly Stream _source0;

            private readonly Stream _source1;

            public int _pos = 0;

            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length => throw new NotSupportedException();

            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public DelayedStream(Stream source0, Stream source1)
            {
                _source0 = source0;
                _source1 = source1;
            }

            public override void Flush() => throw new NotSupportedException();

            public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            {
                if (_pos == 0)
                {
                    var read = await _source0.ReadAsync(buffer, cancellationToken);
                    if (read == 0)
                    {
                        _pos = 1;
                        return await ReadAsync(buffer, cancellationToken);
                    }
                    return read;
                }
                return await _source1.ReadAsync(buffer, cancellationToken);
            }

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
                => ReadAsync(buffer.AsMemory().Slice(offset, count), cancellationToken).AsTask();

            public override int Read(byte[] buffer, int offset, int count)
                => ReadAsync(buffer.AsMemory().Slice(offset, count)).AsTask().Result;

            public override int Read(Span<byte> buffer)
            {
                var buf = ArrayPool<byte>.Shared.Rent(buffer.Length);
                try
                {
                    var size = Read(buf, 0, buf.Length);
                    buf.AsSpan().Slice(0, size).CopyTo(buffer);
                    return size;
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buf);
                }
            }

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();

            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }

        [Fact]
        public async Task CompleteReadTest()
        {
            {
                using var source0 = new MemoryStream(Enumerable.Range(0, 512 * 3).Select(i => (byte)(i % 256)).ToArray(), false);
                using var source1 = new MemoryStream(Enumerable.Range(0, 512 * 3).Select(i => (byte)(i % 256)).ToArray(), false);
                using var stream = new DelayedStream(source0, source1);
                var buffer = new byte[512 * 6];
                var len = await stream.ReadCompleteAsync(buffer);
                Assert.Equal(len, buffer.Length);
                Assert.Equal(buffer, Enumerable.Range(0, 512 * 6).Select(i => (byte)(i % 256)));
            }
            {
                using var source0 = new MemoryStream(Enumerable.Range(0, 512 * 3).Select(i => (byte)(i % 256)).ToArray(), false);
                using var source1 = new MemoryStream(Enumerable.Range(0, 512 * 3).Select(i => (byte)(i % 256)).ToArray(), false);
                using var stream = new DelayedStream(source0, source1);
                var buffer = new byte[512 * 2];
                for (var i = 0; i < 3; ++i)
                {
                    var len = await stream.ReadCompleteAsync(buffer);
                    Assert.Equal(len, buffer.Length);
                    Assert.Equal(buffer, Enumerable.Range(i * 1024, 512 * 2).Select(i => (byte)(i % 256)));
                }
                Assert.Equal(0, await stream.ReadCompleteAsync(buffer));
            }
        }
    }
}
using System;
using System.IO;
using NCoreUtils.SpecializedStreams;
using Xunit;

namespace NCoreUtils
{
    public class ChunkedStreamTests
    {
        private const int ChunkSize = 4 * 1024;

        private static readonly Random _rnd = new Random(unchecked((int)(DateTime.Now.Ticks % int.MaxValue)));

        private static void FillRandom(Span<byte> buffer)
        {
            for (var i = 0; i < buffer.Length; ++i)
            {
                buffer[i] = (byte)(_rnd.Next() % byte.MaxValue);
            }
        }

        private static bool SpanEquals(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }
            for (var i = 0; i < a.Length; ++i)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }

        [Fact]
        public void Primitive()
        {
            using var sink = new ChunkedMemorySinkStream(ChunkSize);
            Assert.False(sink.CanRead);
            Assert.False(sink.CanSeek);
            Assert.False(sink.CanTimeout);
            Assert.True(sink.CanWrite);
            Assert.Equal(0, sink.Position);
            var data = new byte[ChunkSize / 2 + 1];
            sink.Write(data, 0, data.Length);
            sink.WriteAsync(data.AsMemory(), default).AsTask().Wait();
            sink.WriteAsync(data, 0, data.Length, default).Wait();
            Assert.Equal(data.Length * 3, sink.Length);
        }

        [Fact]
        public void HalfChunkSize()
        {
            var data = new byte[ChunkSize / 2];
            FillRandom(data);
            using var sink = new ChunkedMemorySinkStream(ChunkSize);
            sink.Write(data);
            Assert.Equal(data.Length, sink.Position);
            Assert.True(sink.IsSingleChunk);
            Assert.True(sink.TryGetSingleChunk(out var chunk));
            Assert.True(SpanEquals(data, chunk.Span));
            using var accessor = sink.CloseAndCreateAccessor();
            Assert.Throws<ObjectDisposedException>(() => sink.CloseAndCreateAccessor());
            Assert.True(SpanEquals(data, accessor.ToArray()));
            using var buffer = new MemoryStream();
            accessor.CopyTo(buffer);
            Assert.True(SpanEquals(data, buffer.ToArray()));
        }

        [Fact]
        public void ExactChunkSize()
        {
            var data = new byte[ChunkSize];
            FillRandom(data);
            using var sink = new ChunkedMemorySinkStream(ChunkSize);
            sink.Write(data);
            Assert.Equal(data.Length, sink.Position);
            Assert.True(sink.IsSingleChunk);
            Assert.True(sink.TryGetSingleChunk(out var chunk));
            Assert.True(SpanEquals(data, chunk.Span));
            using var accessor = sink.CloseAndCreateAccessor();
            Assert.True(SpanEquals(data, accessor.ToArray()));
            using var buffer = new MemoryStream();
            accessor.CopyTo(buffer);
            Assert.True(SpanEquals(data, buffer.ToArray()));
        }

        [Fact]
        public void OneAndHalfChunkSize()
        {
            var data = new byte[ChunkSize * 3 / 2];
            FillRandom(data);
            using var sink = new ChunkedMemorySinkStream(ChunkSize);
            sink.Write(data);
            Assert.Equal(data.Length, sink.Position);
            Assert.False(sink.IsSingleChunk);
            Assert.False(sink.TryGetSingleChunk(out var chunk));
            using var accessor = sink.CloseAndCreateAccessor();
            Assert.True(SpanEquals(data, accessor.ToArray()));
            using var buffer = new MemoryStream();
            accessor.CopyTo(buffer);
            Assert.True(SpanEquals(data, buffer.ToArray()));
        }
    }
}
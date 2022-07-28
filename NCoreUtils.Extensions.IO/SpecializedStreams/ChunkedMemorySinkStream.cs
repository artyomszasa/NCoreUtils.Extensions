using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreUtils.SpecializedStreams
{
    public sealed class ChunkedMemorySinkStream : Stream
    {
        private readonly MemoryPool<byte> _pool;

        private readonly int _chunkSize;

        private readonly IMemoryOwner<byte> _firstChunk;

        private int _isDisposed;

        private int _position = 0;

        private int _chunkSync = 0;

        private List<IMemoryOwner<byte>>? _chunks;

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanTimeout => false;

        public override bool CanWrite => true;

        public override long Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _position;
        }

        public override long Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _position;
            [ExcludeFromCodeCoverage]
            set => throw new NotSupportedException();
        }

        public bool IsSingleChunk
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _position <= _chunkSize;
        }

        public ChunkedMemorySinkStream(int chunkSize = 32 * 1024, MemoryPool<byte>? pool = default)
        {
            _pool = pool ?? MemoryPool<byte>.Shared;
            _chunkSize = chunkSize;
            _firstChunk = _pool.Rent(chunkSize);
        }

        [Conditional("DEBUG")]
        [ExcludeFromCodeCoverage]
        private void CheckChunkIndex(int index)
        {
            if (index > 0 && (_chunks is null || _chunks.Count < index))
            {
                throw new InvalidOperationException("WTF: no chunk");
            }
        }

        [Conditional("DEBUG")]
        [ExcludeFromCodeCoverage]
        private void CheckAdvanceAmount(int amount)
        {
            if (amount > _chunkSize - (_position % _chunkSize))
            {
                throw new InvalidOperationException("WTF: advising beyond chunk");
            }
        }


        private (Memory<byte> Memory, int Offset) GetCurrentChunk()
        {
            var index = Math.DivRem(_position, _chunkSize, out var offset);
            // the following should never happen --> debugging only
            CheckChunkIndex(index);
            return (
                (0 == index ? _firstChunk : _chunks![index - 1]).Memory,
                offset
            );
        }

        private void Advance(int amount)
        {
            // the following should never happen --> debugging only
            CheckAdvanceAmount(amount);
            // avoid concurrent position changes: though this can only happen when stream is used incorrectly we can
            // handle it anyway...
            bool success;
            do
            {
                var position0 = _position;
                Interlocked.MemoryBarrier();
                var position1 = position0 + amount;
                success = position0 == Interlocked.CompareExchange(ref _position, position1, position0);
            }
            while (!success);
            var chunkCount = _position / _chunkSize;
            if (chunkCount > 0)
            {
                // lock
                while (0 != Interlocked.CompareExchange(ref _chunkSync, 1, 0)) { }
                if (_chunks is null)
                {
                    _chunks = new List<IMemoryOwner<byte>>(16);
                }
                while (_chunks.Count < chunkCount)
                {
                    _chunks.Add(_pool.Rent(_chunkSize));
                }
                // unlock
                Interlocked.CompareExchange(ref _chunkSync, 0, 1);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (0 == Interlocked.CompareExchange(ref _isDisposed, 1, 0))
            {
                if (disposing)
                {
                    _firstChunk.Dispose();
                    if (_chunks is not null)
                    {
                        foreach (var chunk in _chunks)
                        {
                            chunk.Dispose();
                        }
                        _chunks = default;
                    }
                }
            }
            base.Dispose(disposing);
        }

        [ExcludeFromCodeCoverage]
        public override void Flush() { }

        [ExcludeFromCodeCoverage]
        public override Task FlushAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        [ExcludeFromCodeCoverage]
        public override int Read(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();

        [ExcludeFromCodeCoverage]
        public override long Seek(long offset, SeekOrigin origin)
            => throw new NotSupportedException();

        [ExcludeFromCodeCoverage]
        public override void SetLength(long value)
            => throw new NotSupportedException();

        public
#if NETSTANDARD2_1 || NET6_0_OR_GREATER
            override
#endif
            void Write(ReadOnlySpan<byte> buffer)
        {
            var (memory, offset) = GetCurrentChunk();
            var free = _chunkSize - offset;
            if (buffer.Length > free)
            {
                Advance(free);
                buffer[..free].CopyTo(memory.Span[offset..]);
                Write(buffer[free..]);
            }
            else
            {
                Advance(buffer.Length);
                buffer.CopyTo(memory.Span[offset..]);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
            => Write(buffer.AsSpan().Slice(offset, count));

#if NETSTANDARD2_1 || NET6_0_OR_GREATER
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            Write(buffer.Span);
            return default;
        }
#endif

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Write(buffer.AsSpan().Slice(offset, count));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Tries to get written content as single chunk. Only succeeds when the length of the content written is less
        /// or equals to the chunk size.
        /// <para>
        /// IMPORTANT: the memory returned through the <paramref name="chunk" /> parameter is owned by the instance, it
        /// must not be accessed outside the scope of the instance!
        /// </para>
        /// </summary>
        /// <param name="chunk">
        /// On success the single continous memory block containing the contents written to the stream.
        /// </param>
        /// <returns>
        /// <c>true</c> when the length of the content written is less or equals to the chunk size and can be
        /// represented by a single continous memory block, <c>false</c> otherwise.
        /// </returns>
        public bool TryGetSingleChunk(out Memory<byte> chunk)
        {
            if (IsSingleChunk)
            {
                chunk = _firstChunk.Memory[.._position];
                return true;
            }
            chunk = Memory<byte>.Empty;
            return false;
        }

        /// <summary>
        /// Effectively disposes the stream and creates another stream to access the contents of the actual stream
        /// instance. This means that the actual instance must not be used after this method is invoked.
        /// </summary>
        /// <returns>
        /// New readable stream that contains contents of the actual stream instance.
        /// </returns>
        public ChunkedMemoryAccessStream CloseAndCreateAccessor()
        {
            if (0 == Interlocked.CompareExchange(ref _isDisposed, 1, 0))
            {
                var chunks = _chunks;
                _chunks = default;
                chunks ??= new List<IMemoryOwner<byte>>(1);
                chunks.Insert(0, _firstChunk);
                return new ChunkedMemoryAccessStream(chunks, _chunkSize, _position);
            }
            else
            {
                throw new ObjectDisposedException(nameof(ChunkedMemorySinkStream));
            }
        }
    }
}
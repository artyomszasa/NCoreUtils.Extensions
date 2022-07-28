using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreUtils.SpecializedStreams
{
    public sealed class ChunkedMemoryAccessStream : Stream
    {
        private readonly IReadOnlyList<IMemoryOwner<byte>> _chunks;

        private readonly int _chunkSize;

        private readonly int _length;

        private int _position = 0;

        private int _isDisposed = 0;

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanTimeout => false;

        public override bool CanWrite => false;

        public override long Length => _length;

        public override long Position
        {
            get => _position;
            set
            {
                if (value <= _length)
                {
                    _position = unchecked((int)value);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        internal ChunkedMemoryAccessStream(
            IReadOnlyList<IMemoryOwner<byte>> chunks,
            int chunkSize,
            int length)
        {
            // NOTE: no null checks as this is an internal ctor.
            _chunks = chunks;
            _chunkSize = chunkSize;
            _length = length;
        }

        private void Advance(int amount)
        {
            // the following should never happen --> debugging only
            #if DEBUG
            if (amount + _position > _length)
            {
                throw new InvalidOperationException("WTF: advancing beyond eos");
            }
            #endif
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
        }

        protected override void Dispose(bool disposing)
        {
            if (0 == Interlocked.CompareExchange(ref _isDisposed, 1, 0))
            {
                if (disposing)
                {
                    foreach (var chunk in _chunks)
                    {
                        chunk.Dispose();
                    }
                }
            }
            base.Dispose(disposing);
        }

        [ExcludeFromCodeCoverage]
        public override void Flush()
            => throw new NotSupportedException();

        public
#if NETSTANDARD2_1 || NET6_0_OR_GREATER
            override
#endif
            int Read(Span<byte> buffer)
        {
            var position = _position;
            var readable = _length - _position;
            if (readable <= 0)
            {
                return 0;
            }
            var toRead = Math.Min(buffer.Length, readable);
            Advance(toRead);
            var stored = 0;
            while (stored < toRead)
            {
                // get chunk
                var index = Math.DivRem(position, _chunkSize, out var offset);
                var available = _chunkSize - offset;
                var toCopy = Math.Min(available, toRead - stored);
                _chunks[index].Memory.Span.Slice(offset, toCopy).CopyTo(buffer[stored..]);
                stored += toCopy;
                position += toCopy;
            }
            return stored;
        }

        public override int Read(byte[] buffer, int offset, int count)
            => Read(buffer.AsSpan().Slice(offset, count));

#if NETSTANDARD2_1 || NET6_0_OR_GREATER
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            => new(Read(buffer.Span));
#endif

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => Task.FromResult(Read(buffer.AsSpan(offset, count)));


        public override long Seek(long offset, SeekOrigin origin)
        {
            var position = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => _position + offset,
                SeekOrigin.End => _length - offset,
                _ => throw new ArgumentException("Invalid seek origin.", nameof(origin))
            };
            if (position < 0L || position > _length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            _position = unchecked((int)position);
            return position;
        }

        [ExcludeFromCodeCoverage]
        public override void SetLength(long value)
            => throw new NotSupportedException();

        [ExcludeFromCodeCoverage]
        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();

        public byte[] ToArray()
        {
            var result = new byte[_length];
            var position = 0;
            while (position < _length)
            {
                var index = position / _chunkSize;
                var toCopy = Math.Min(_length - position, _chunkSize);
                _chunks[index].Memory.Span[..toCopy].CopyTo(result.AsSpan(position));
                position += toCopy;
            }
            return result;
        }
    }
}
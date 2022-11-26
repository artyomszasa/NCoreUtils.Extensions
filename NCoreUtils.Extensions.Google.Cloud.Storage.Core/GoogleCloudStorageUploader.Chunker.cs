using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreUtils;

public partial class GoogleCloudStorageUploader
{
    private interface IDataChunkSource : IDisposable, IAsyncDisposable
    {
        ValueTask<(int Read, bool Final)> FetchChunkAsync(Memory<byte> buffer, CancellationToken cancellationToken);
    }

    private sealed class DefaultChunkSource : IDataChunkSource
    {
        private Stream Source { get; }

        private bool LeaveOpen { get; }

        private long TotalFetched { get; set; }

        public DefaultChunkSource(Stream source, bool leaveOpen)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            LeaveOpen = leaveOpen;
        }

        public async ValueTask<(int Read, bool Final)> FetchChunkAsync(Memory<byte> buffer, CancellationToken cancellationToken)
        {
            var totalRead = 0;
            do
            {
                var read = await Source.ReadAsync(buffer[totalRead..], cancellationToken).ConfigureAwait(false);
                if (0 == read)
                {
                    // out of source data while fetching chunk --> chunk is a final chunk.
                    TotalFetched += totalRead;
                    return (totalRead, true);
                }
                totalRead += read;
            }
            while (totalRead < buffer.Length);
            TotalFetched += totalRead;
            return (totalRead, TotalFetched == Source.Length);
        }

        public void Dispose()
        {
            if (!LeaveOpen)
            {
                Source.Dispose();
            }
        }

        public ValueTask DisposeAsync()
            => LeaveOpen ? default : Source.DisposeAsync();
    }

    private sealed class PrefetchChunkSource : IDataChunkSource
    {
        private const int PrefetchBufferSize = 16 * 1024;

        private byte[]? PrefetchBuffer { get; set; }

        private Stream Source { get; }

        private bool LeaveOpen { get; }

        private int Prefetched { get; set; }

        public PrefetchChunkSource(Stream source, bool leaveOpen)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            LeaveOpen = leaveOpen;
        }

        public async ValueTask<(int Read, bool Final)> FetchChunkAsync(
            Memory<byte> buffer,
            CancellationToken cancellationToken)
        {
            var totalRead = 0;
            if (PrefetchBuffer is not null && Prefetched > 0)
            {
                if (Prefetched > buffer.Length)
                {
                    PrefetchBuffer.AsSpan(0, buffer.Length).CopyTo(buffer.Span);
                    var remaining = Prefetched - buffer.Length;
                    PrefetchBuffer.AsSpan(buffer.Length, remaining).CopyTo(PrefetchBuffer.AsSpan());
                    Prefetched = remaining;
                    return (buffer.Length, false);
                }
                else
                {
                    PrefetchBuffer.AsSpan(0, Prefetched).CopyTo(buffer.Span);
                    totalRead += Prefetched;
                    Prefetched = 0;
                }
            }
            // FIXME: DEBUG assert (Prefetched == 0)
            while (totalRead < buffer.Length)
            {
                var read = await Source.ReadAsync(buffer[totalRead..], cancellationToken).ConfigureAwait(false);
                if (0 == read)
                {
                    // out of source data while fetching chunk --> chunk is a final chunk.
                    return (totalRead, true);
                }
                totalRead += read;
            };
            // there may be more data, that must be prefetched to check whether the chunk is final
            PrefetchBuffer ??= ArrayPool<byte>.Shared.Rent(PrefetchBufferSize);
            Prefetched = await Source.ReadAsync(PrefetchBuffer.AsMemory(), cancellationToken).ConfigureAwait(false);
            return (totalRead, Prefetched > 0);
        }

        public void Dispose()
        {
            if (!LeaveOpen)
            {
                Source.Dispose();
            }
            var prefetchBuffer = PrefetchBuffer;
            Prefetched = 0;
            PrefetchBuffer = null;
            if (prefetchBuffer is not null)
            {
                ArrayPool<byte>.Shared.Return(prefetchBuffer);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (!LeaveOpen)
            {
                await Source.DisposeAsync().ConfigureAwait(false);
            }
            var prefetchBuffer = PrefetchBuffer;
            Prefetched = 0;
            PrefetchBuffer = null;
            if (prefetchBuffer is not null)
            {
                ArrayPool<byte>.Shared.Return(prefetchBuffer);
            }
        }
    }

    private static IDataChunkSource CreateChunkSource(Stream stream, bool leaveOpen)
    {
        try
        {
            _ = stream.Length;
            // if stream has length --> use the length
            return new DefaultChunkSource(stream, leaveOpen);
        }
        catch
        {
            // if stream has no length --> use prefetch
            return new PrefetchChunkSource(stream, leaveOpen);
        }
    }
}
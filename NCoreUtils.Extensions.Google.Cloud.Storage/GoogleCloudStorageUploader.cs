using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NCoreUtils.Google;

namespace NCoreUtils
{
    public sealed class GoogleCloudStorageUploader : IDisposable, IAsyncDisposable
    {
        private sealed class SliceMemoryOwner : IMemoryOwner<byte>
        {
            private readonly IMemoryOwner<byte> _source;

            private readonly int _size;

            public SliceMemoryOwner(IMemoryOwner<byte> source, int size)
            {
                _source = source ?? throw new ArgumentNullException(nameof(source));
                if (source.Memory.Length < size)
                {
                    throw new ArgumentException(nameof(source), "Supplied memory is smaller than the requested slice size.");
                }
                _size = size;
            }

            public Memory<byte> Memory
                => _source.Memory.Slice(0, _size);

            public void Dispose()
                => _source.Dispose();
        }

        public static int MinChunkSize { get; } = 256 * 1024;

        public static int DefaultChunkSize { get; } = 512 * 1024;

        // readonly UnmanagedMemoryManager<byte> _buffer = new UnmanagedMemoryManager<byte>(512 * 1024);

        private readonly IMemoryOwner<byte> _buffer;

        private long _sent;

        private long _isDisposed;

        public event EventHandler<GoogleCloudStorageUploaderProgressArgs>? Progress;

        private long Sent
        {
            get => _sent;
            set
            {
                if (_sent != value)
                {
                    _sent = value;
                    Progress?.Invoke(this, new GoogleCloudStorageUploaderProgressArgs(value));
                }
            }
        }

        public HttpClient Client { get; }

        public Uri EndPoint { get; }

        public MediaTypeHeaderValue ContentType { get; }

        public GoogleCloudStorageUploader(
            HttpClient client,
            Uri endpoint,
            string? contentType = default,
            IMemoryOwner<byte>? buffer = default)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            EndPoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            ContentType = MediaTypeHeaderValue.Parse(contentType ?? "application/octet-stream");
            if (buffer is null)
            {
                var b = MemoryPool<byte>.Shared.Rent(DefaultChunkSize);
                if (b.Memory.Length == DefaultChunkSize)
                {
                    _buffer = b;
                }
                else
                {
                    _buffer = new SliceMemoryOwner(b, DefaultChunkSize);
                }
            }
            else
            {
                var memoryLength = buffer.Memory.Length;
                if (memoryLength < MinChunkSize)
                {
                    throw new InvalidOperationException($"Supplied memory is smaller than the minimum chunk size ({MinChunkSize}).");
                }
                var usableSize = (memoryLength / MinChunkSize) * MinChunkSize;
                if (memoryLength == usableSize)
                {
                    _buffer = buffer;
                }
                else
                {
                    _buffer = new SliceMemoryOwner(buffer, usableSize);
                }
            }
        }

        public GoogleCloudStorageUploader(
            HttpClient client,
            string endpoint,
            string? contentType = default,
            IMemoryOwner<byte>? buffer = default)
            : this(client, new Uri(endpoint, UriKind.Absolute), contentType, buffer)
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfDisposed()
        {
            if (0 != Interlocked.CompareExchange(ref _isDisposed, 0, 0))
            {
                throw new ObjectDisposedException(nameof(GoogleCloudStorageUploader));
            }
        }

        private async Task<HttpResponseMessage> SendChunkAsync(int size, bool final, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var targetSize = Sent + size;
            var content = new ReadOnlyMemoryContent(size == _buffer.Memory.Length ? _buffer.Memory : _buffer.Memory.Slice(0, size));
            var headers = content.Headers;
            headers.ContentLength = size;
            headers.ContentType = ContentType;
            headers.ContentRange = !final ? new ContentRangeHeaderValue(Sent, targetSize - 1L) : new ContentRangeHeaderValue(Sent, targetSize - 1L, targetSize);
            using var request = new HttpRequestMessage(HttpMethod.Put, EndPoint) { Content = content };
            return await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        }

        async ValueTask<int> FillBuffer(Stream stream, CancellationToken cancellationToken)
        {
            var totalRead = 0;
            int read;
            do
            {
                read = await stream.ReadAsync(_buffer.Memory.Slice(totalRead, _buffer.Memory.Length - totalRead), cancellationToken).ConfigureAwait(false);
                totalRead += read;
            }
            while (read > 0 && totalRead < _buffer.Memory.Length);
            return totalRead;
        }

        public async Task ConumeStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            var read = await FillBuffer(stream, cancellationToken).ConfigureAwait(false);
            if (read == _buffer.Memory.Length)
            {
                // partial upload
                var response = await SendChunkAsync(read, false, cancellationToken).ConfigureAwait(false);
                if ((int)response.StatusCode == 308)
                {
                    // chunk upload successfull (TODO: check if response range is set properly)
                    Sent += read;
                    await ConumeStreamAsync(stream, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    throw new GoogleCloudStorageUploadException($"Upload chunk failed with status code {response.StatusCode}.");
                }
            }
            else
            {
                // FIXME: handle stream being divisable by chunk size
                var response = await SendChunkAsync(read, true, cancellationToken).ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
                {
                    throw new GoogleCloudStorageUploadException($"Upload final chunk failed with status code {response.StatusCode}.");
                }
                Sent += read;
            }
        }

        public void Dispose()
        {
            if (0 == Interlocked.CompareExchange(ref _isDisposed, 1, 0))
            {
                _buffer.Dispose();
                Client.Dispose();
            }
        }

        public ValueTask DisposeAsync()
        {
            if (0 == Interlocked.CompareExchange(ref _isDisposed, 1, 0))
            {
                _buffer.Dispose();
                Client.Dispose();
            }
            return default;
        }
    }
}
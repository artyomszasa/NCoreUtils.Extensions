using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NCoreUtils.Google;

namespace NCoreUtils;

public sealed partial class GoogleCloudStorageUploader : IDisposable, IAsyncDisposable
{
    private static MediaTypeHeaderValue? _applicationOctetStream;

    private static MediaTypeHeaderValue ApplicationOctetStream
        => _applicationOctetStream ??= MediaTypeHeaderValue.Parse("application/octet-stream");

    public static int MinChunkSize { get; } = 256 * 1024;

    public static int DefaultChunkSize { get; } = 512 * 1024;

    private static string FormatGoogleError(GoogleErrorResponse response)
    {
        if (response is null || response.Error is null)
        {
            return string.Empty;
        }
        var buffer = ArrayPool<char>.Shared.Rent(16 * 1024);
        try
        {
            var builder = new SpanBuilder(buffer);
            builder.Append(response.Error.Code);
            builder.Append(": ");
            builder.Append(response.Error.Message);
            if (response.Error.Errors is { Count: >0 } errors)
            {
                builder.Append(" => ");
                foreach (var error in errors)
                {
                    builder.Append(error, GoogleErrorDetailsEmplacer);
                }
            }
            return builder.ToString();
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private static async Task<GoogleErrorResponse?> ReadErrorResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = await response.Content
                .ReadAsStreamAsync(cancellationToken)
                .ConfigureAwait(false);
            return await JsonSerializer.DeserializeAsync(stream, GoogleJsonContext.Default.GoogleErrorResponse, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exn)
        {
            Console.WriteLine(exn); // FIXME: use logging
            return default;
        }
    }

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
        ContentType = string.IsNullOrEmpty(contentType)
            ? ApplicationOctetStream
            : MediaTypeHeaderValue.Parse(contentType);
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
            var unalignedSize = memoryLength % MinChunkSize;
            if (unalignedSize == 0)
            {
                _buffer = buffer;
            }
            else
            {
                var usableSize = memoryLength - unalignedSize;
                _buffer = new SliceMemoryOwner(buffer, usableSize);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        var content = new ReadOnlyMemoryContent(size == _buffer.Memory.Length ? _buffer.Memory : _buffer.Memory[..size]);
        var headers = content.Headers;
        headers.ContentLength = size;
        headers.ContentType = ContentType;
        headers.ContentRange = !final ? new ContentRangeHeaderValue(Sent, targetSize - 1L) : new ContentRangeHeaderValue(Sent, targetSize - 1L, targetSize);
        using var request = new HttpRequestMessage(HttpMethod.Put, EndPoint) { Content = content };
        return await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<int> FillBuffer(Stream stream, CancellationToken cancellationToken)
    {
        var totalRead = 0;
        int read;
        do
        {
            read = await stream.ReadAsync(_buffer.Memory[totalRead..], cancellationToken).ConfigureAwait(false);
            totalRead += read;
        }
        while (read > 0 && totalRead < _buffer.Memory.Length);
        return totalRead;
    }

    private static Memory.SpanEmplaceableEmplacer<GoogleErrorDetails> GoogleErrorDetailsEmplacer { get; } = new();

    public async Task UploadAsync(Stream stream, bool leaveOpen, CancellationToken cancellationToken)
    {
        await using var chunker = CreateChunkSource(stream, leaveOpen);
        while (true)
        {
            var (size, final) = await chunker.FetchChunkAsync(_buffer.Memory, cancellationToken).ConfigureAwait(false);
            if (size == 0)
            {
                throw new InvalidOperationException("Should never happen: stream is empty?");
            }
            try
            {
                using var response = await SendChunkAsync(size, final, cancellationToken).ConfigureAwait(false);
                if (final)
                {
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
                    {
                        var error = await ReadErrorResponseAsync(response, CancellationToken.None).ConfigureAwait(false)
                            ?? throw new GoogleCloudStorageUploadException($"Upload final chunk failed with status code {response.StatusCode} [no error description].");
                        throw new GoogleCloudStorageUploadException($"Upload final chunk failed with status code {response.StatusCode} [{FormatGoogleError(error)}].");
                    }
                    // final chunk upload successfull
                    Sent += size;
                    return;
                }
                if (response.StatusCode != HttpStatusCode.PermanentRedirect)
                {
                    var error = await ReadErrorResponseAsync(response, CancellationToken.None).ConfigureAwait(false)
                        ?? throw new GoogleCloudStorageUploadException($"Upload chunk failed with status code {response.StatusCode} [no error description].");
                    throw new GoogleCloudStorageUploadException($"Upload chunk failed with status code {response.StatusCode} [{FormatGoogleError(error)}].");
                }
                // chunk upload successfull (TODO: check if response range is set properly)
                Sent += size;
            }
            catch (Exception exn)
            {
                // NOTE: Xamarin AndroidMessageHandler throws exception on 308 response
                // see: https://github.com/xamarin/xamarin-android/issues/4477
                if (!final && exn.Message.Contains("not supported") && exn.Message.Contains("308"))
                {
                    // chunk upload successfull
                    Sent += size;
                }
                else
                {
                    throw;
                }
            }
        }
    }

    [Obsolete("Use UploadAsync instead")]
    public async Task ConumeStreamAsync(Stream stream, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
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
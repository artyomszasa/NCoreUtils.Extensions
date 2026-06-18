using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using NCoreUtils.Google;

namespace NCoreUtils.Internal;

public abstract partial class ResumableUploader<TResult> : ResumableUploader, IDisposable, IAsyncDisposable
{
    private static MediaTypeHeaderValue? _applicationOctetStream;

    protected static MediaTypeHeaderValue ApplicationOctetStream
        => _applicationOctetStream ??= MediaTypeHeaderValue.Parse("application/octet-stream");

    public static int MinChunkSize { get; } = 256 * 1024;

    public static int DefaultChunkSize { get; } = 512 * 1024;

    protected static async Task<GoogleErrorResponse?> ReadErrorResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
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

    private int _isDisposed;

    public event EventHandler<ResumableUploaderProgressArgs>? UploadProgress;

    private long Sent
    {
        get => _sent;
        set
        {
            if (_sent != value)
            {
                _sent = value;
                OnUploadProgress(value);
            }
        }
    }

    public HttpClient Client { get; }

    public Uri EndPoint { get; }

    public MediaTypeHeaderValue ContentType { get; }

    public ResumableUploader(
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
    public ResumableUploader(
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
            throw new ObjectDisposedException(GetType().Name);
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

    // private static Memory.SpanEmplaceableEmplacer<GoogleErrorDetails> GoogleErrorDetailsEmplacer { get; } = new();

    protected virtual void OnUploadProgress(long value)
    {
        UploadProgress?.Invoke(this, new ResumableUploaderProgressArgs(value));
    }

    [DoesNotReturn]
    protected virtual async ValueTask ThrowOnResponseFailure(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var error = await ReadErrorResponseAsync(response, CancellationToken.None).ConfigureAwait(false)
            ?? throw new ResumableUploadException($"Upload final chunk failed with status code {response.StatusCode} [no error description].");
        throw new ResumableUploadException($"Upload final chunk failed with status code {response.StatusCode}.", error?.Error);
    }

    [DoesNotReturn]
    protected virtual void ThrowOnChunkUploadFailure(HttpResponseMessage response, bool final)
    {
        if (final)
        {
            throw new ResumableUploadException($"Upload final chunk failed with status code {response.StatusCode}.");
        }
        throw new ResumableUploadException($"Upload chunk failed with status code {response.StatusCode}.");
    }

    protected abstract ValueTask<TResult> ProcessFinalResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken);

    public async Task<TResult> UploadAsync(Stream stream, bool leaveOpen, CancellationToken cancellationToken)
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
                        await ThrowOnResponseFailure(response, CancellationToken.None);
                    }
                    // final chunk upload successfull
                    Sent += size;
                    return await ProcessFinalResponseAsync(response, cancellationToken).ConfigureAwait(false);
                }
                if (response.StatusCode != HttpStatusCode.PermanentRedirect)
                {
                    await ThrowOnResponseFailure(response, CancellationToken.None);
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

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && 0 == Interlocked.CompareExchange(ref _isDisposed, 1, 0))
        {
            _buffer.Dispose();
            Client.Dispose();
        }
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        if (0 == Interlocked.CompareExchange(ref _isDisposed, 1, 0))
        {
            _buffer.Dispose();
            Client.Dispose();
        }
        return default;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }
}
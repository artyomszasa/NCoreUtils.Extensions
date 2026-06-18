using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using NCoreUtils.Google;
using NCoreUtils.Internal;

namespace NCoreUtils;

public sealed partial class GoogleCloudStorageUploader(
    HttpClient client,
    Uri endpoint,
    string? contentType = default,
    IMemoryOwner<byte>? buffer = default)
    : ResumableUploader<int>(client, endpoint, contentType, buffer)
{
    [Obsolete("Use ResumableUploader.UploadProgress instead.")]
    public event EventHandler<GoogleCloudStorageUploaderProgressArgs>? Progress;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GoogleCloudStorageUploader(
        HttpClient client,
        string endpoint,
        string? contentType = default,
        IMemoryOwner<byte>? buffer = default)
        : this(client, new Uri(endpoint, UriKind.Absolute), contentType, buffer)
    { }

    protected override void OnUploadProgress(long value)
    {
        base.OnUploadProgress(value);
#pragma warning disable CS0618 // Type or member is obsolete
        Progress?.Invoke(this, new GoogleCloudStorageUploaderProgressArgs(value));
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [DoesNotReturn]
    protected override async ValueTask ThrowOnResponseFailure(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var error = await ReadErrorResponseAsync(response, CancellationToken.None).ConfigureAwait(false)
            ?? throw new GoogleCloudStorageUploadException($"Upload final chunk failed with status code {response.StatusCode} [no error description].");
        throw new GoogleCloudStorageUploadException($"Upload final chunk failed with status code {response.StatusCode}.", error?.Error);
    }

    [DoesNotReturn]
    protected override void ThrowOnChunkUploadFailure(HttpResponseMessage response, bool final)
    {
        if (final)
        {
            throw new GoogleCloudStorageUploadException($"Upload final chunk failed with status code {response.StatusCode}.");
        }
        throw new GoogleCloudStorageUploadException($"Upload chunk failed with status code {response.StatusCode}.");
    }

    protected override ValueTask<int> ProcessFinalResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        => new((int)response.StatusCode);

    public new Task UploadAsync(Stream stream, bool leaveOpen, CancellationToken cancellationToken)
        => base.UploadAsync(stream, leaveOpen, cancellationToken);
}
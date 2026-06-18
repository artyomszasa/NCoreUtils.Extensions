using System.Buffers;
using System.Net.Http.Json;
using NCoreUtils.Internal;

namespace NCoreUtils.Google;

public partial class DriveClient
{
    private sealed class DriveResumableUploader(
        HttpClient client,
        Uri endpoint,
        string? contentType = null,
        IMemoryOwner<byte>? buffer = null)
        : ResumableUploader<Drive.File>(client, endpoint, contentType, buffer)
    {
        public DriveResumableUploader(
            HttpClient client,
            string endpoint,
            string? contentType = null,
            IMemoryOwner<byte>? buffer = null)
            : this(client, new Uri(endpoint, UriKind.Absolute), contentType, buffer)
        { }

        protected override async ValueTask<Drive.File> ProcessFinalResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            return await response.Content.ReadFromJsonAsync(DriveApiV3SerializerContext.Default.File, cancellationToken)
                ?? throw new InvalidOperationException("Final response returned no File metadata.");
        }
    }

    private HttpClient CreateHttpClient()
    {
        if (httpClientFactory is not null)
        {
            return httpClientFactory.CreateClient(nameof(DriveClient));
        }
        return new HttpClient();
    }

    private async Task<Drive.File> SimpleUploadFileAsync(
        bool? ignoreDefaultVisibility,
        bool? keepRevisionForever,
        string? ocrLanguage,
        bool? supportsAllDrives,
        bool? useContentAsIndexableText,
        string? includePermissionsForView,
        string? includeLabels,
        Stream stream,
        bool leaveStreamOpen,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await api.SimpleUploadFileAsync(
                ignoreDefaultVisibility: ignoreDefaultVisibility,
                keepRevisionForever: keepRevisionForever,
                ocrLanguage: ocrLanguage,
                supportsAllDrives: supportsAllDrives,
                useContentAsIndexableText: useContentAsIndexableText,
                includePermissionsForView: includePermissionsForView,
                includeLabels: includeLabels,
                stream: stream,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
        }
        finally
        {
            if (!leaveStreamOpen)
            {
                await stream.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    private async Task<Drive.File> MultipartUploadFileAsync(
        bool? ignoreDefaultVisibility,
        bool? keepRevisionForever,
        string? ocrLanguage,
        bool? supportsAllDrives,
        bool? useContentAsIndexableText,
        string? includePermissionsForView,
        string? includeLabels,
        Drive.File file,
        Stream stream,
        bool leaveStreamOpen,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await api.MultipartUploadFileAsync(
                ignoreDefaultVisibility: ignoreDefaultVisibility,
                keepRevisionForever: keepRevisionForever,
                ocrLanguage: ocrLanguage,
                supportsAllDrives: supportsAllDrives,
                useContentAsIndexableText: useContentAsIndexableText,
                includePermissionsForView: includePermissionsForView,
                includeLabels: includeLabels,
                file: file,
                stream: stream,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
        }
        finally
        {
            if (!leaveStreamOpen)
            {
                await stream.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    private async Task<Drive.File> ResumableUploadAsync(
        bool? ignoreDefaultVisibility,
        bool? keepRevisionForever,
        string? ocrLanguage,
        bool? supportsAllDrives,
        bool? useContentAsIndexableText,
        string? includePermissionsForView,
        string? includeLabels,
        Drive.File? file,
        Stream stream,
        bool leaveStreamOpen,
        CancellationToken cancellationToken)
    {
        // initialize resumable upload
        var endpoint = await api.InitializeResumableUploadAsync(
            ignoreDefaultVisibility: ignoreDefaultVisibility,
            keepRevisionForever: keepRevisionForever,
            ocrLanguage: ocrLanguage,
            supportsAllDrives: supportsAllDrives,
            useContentAsIndexableText: useContentAsIndexableText,
            includePermissionsForView: includePermissionsForView,
            includeLabels: includeLabels,
            file: file,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        // configure uploader
        await using var uploader = new DriveResumableUploader(
            CreateHttpClient(),
            endpoint,
            contentType: file?.MimeType
        );
        // upload object
        return await uploader.UploadAsync(stream, leaveStreamOpen, cancellationToken).ConfigureAwait(false);
    }
}
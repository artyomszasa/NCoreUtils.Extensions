using System.Runtime.CompilerServices;
using NCoreUtils.Google.Drive;

namespace NCoreUtils.Google;

public partial class DriveClient(IDriveApiV3 api, IHttpClientFactory? httpClientFactory)
    : IDriveClient
{
    public Task<Drive.File> CreateFileAsync(
        bool? ignoreDefaultVisibility = null,
        bool? keepRevisionForever = null,
        string? ocrLanguage = null,
        bool? supportsAllDrives = null,
        bool? useContentAsIndexableText = null,
        string? includePermissionsForView = null,
        string? includeLabels = null,
        Drive.File? file = null,
        Stream? stream = null,
        bool leaveStreamOpen = false,
        CancellationToken cancellationToken = default)
    {
        if (stream is null)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file), "Either file or stream must be specified.");
            }
            return api.CreateMetadataFileAsync(
                ignoreDefaultVisibility: ignoreDefaultVisibility,
                keepRevisionForever: keepRevisionForever,
                supportsAllDrives: supportsAllDrives,
                includePermissionsForView: includePermissionsForView,
                includeLabels: includeLabels,
                file: file,
                cancellationToken: cancellationToken
            );
        }
        long? dataLength;
        try { dataLength = stream.Length; } catch { dataLength = null; }
        if (dataLength is long length0 && length0 < 256L * 1024L)
        {
            // use multipart/simple upload
            if (file is null)
            {
                // use simple upload
                return SimpleUploadFileAsync(
                    ignoreDefaultVisibility: ignoreDefaultVisibility,
                    keepRevisionForever: keepRevisionForever,
                    ocrLanguage: ocrLanguage,
                    supportsAllDrives: supportsAllDrives,
                    useContentAsIndexableText: useContentAsIndexableText,
                    includePermissionsForView: includePermissionsForView,
                    includeLabels: includeLabels,
                    stream: stream,
                    leaveStreamOpen: leaveStreamOpen,
                    cancellationToken: cancellationToken
                );
            }
            // use multipart upload
            return MultipartUploadFileAsync(
                ignoreDefaultVisibility: ignoreDefaultVisibility,
                keepRevisionForever: keepRevisionForever,
                ocrLanguage: ocrLanguage,
                supportsAllDrives: supportsAllDrives,
                useContentAsIndexableText: useContentAsIndexableText,
                includePermissionsForView: includePermissionsForView,
                includeLabels: includeLabels,
                file: file,
                stream: stream,
                leaveStreamOpen: leaveStreamOpen,
                cancellationToken: cancellationToken
            );
        }
        return ResumableUploadAsync(
            ignoreDefaultVisibility: ignoreDefaultVisibility,
            keepRevisionForever: keepRevisionForever,
            ocrLanguage: ocrLanguage,
            supportsAllDrives: supportsAllDrives,
            useContentAsIndexableText: useContentAsIndexableText,
            includePermissionsForView: includePermissionsForView,
            includeLabels: includeLabels,
            file: file,
            stream: stream,
            leaveStreamOpen: leaveStreamOpen,
            cancellationToken: cancellationToken
        );
    }

    public async IAsyncEnumerable<MinimalFileInfo> ListMinimalFilesAsync(
        string? corpora = null,
        string? driveId = null,
        bool? includeItemsFromAllDrives = null,
        string? orderBy = null,
        uint? maxResults = null,
        string? q = null,
        string? spaces = null,
        bool? supportsAllDrives = null,
        string? includePermissionsForView = null,
        string? includeLabels = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var emitted = 0u;
        string? pageToken = default;
        while (emitted < maxResults)
        {
            var desiredCount = maxResults is uint max
                ? Math.Min(100u, max - emitted)
                : 100u;
            var resp = await api.ListMinimalFilesAsync(
                corpora: corpora,
                driveId: driveId,
                includeItemsFromAllDrives: includeItemsFromAllDrives,
                orderBy: orderBy,
                pageSize: desiredCount,
                pageToken: pageToken,
                q: q,
                spaces: spaces,
                supportsAllDrives: supportsAllDrives,
                includePermissionsForView: includePermissionsForView,
                includeLabels: includeLabels,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
            if (resp.Files is { Count: > 0 } files)
            {
                foreach (var file in files)
                {
                    yield return file;
                    if (++emitted >= maxResults)
                    {
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(resp.NextPageToken))
            {
                break;
            }
            pageToken = resp.NextPageToken;
        }
    }
}
using NCoreUtils.Google.Drive;

namespace NCoreUtils;

public interface IDriveClient
{
    IAsyncEnumerable<MinimalFileInfo> ListMinimalFilesAsync(
        string? corpora = default,
        string? driveId = default,
        bool? includeItemsFromAllDrives = default,
        string? orderBy = default,
        uint? maxResults = default,
        string? q = default,
        string? spaces = default,
        bool? supportsAllDrives = default,
        string? includePermissionsForView = default,
        string? includeLabels = default,
        CancellationToken cancellationToken = default
    );

    Task<Google.Drive.File> CreateFileAsync(
        bool? ignoreDefaultVisibility = default,
        bool? keepRevisionForever = default,
        string? ocrLanguage = default,
        bool? supportsAllDrives = default,
        bool? useContentAsIndexableText = default,
        string? includePermissionsForView = default,
        string? includeLabels = default,
        Google.Drive.File? file = default,
        Stream? stream = default,
        bool leaveStreamOpen = false,
        CancellationToken cancellationToken = default
    );
}
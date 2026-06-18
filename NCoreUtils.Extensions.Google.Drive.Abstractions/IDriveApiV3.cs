namespace NCoreUtils.Google.Drive;

public interface IDriveApiV3
{
    Task<ListMinimalFilesResponse> ListMinimalFilesAsync(
        string? corpora = default,
        string? driveId = default,
        bool? includeItemsFromAllDrives = default,
        string? orderBy = default,
        uint? pageSize = default,
        string? pageToken = default,
        string? q = default,
        string? spaces = default,
        bool? supportsAllDrives = default,
        string? includePermissionsForView = default,
        string? includeLabels = default,
        CancellationToken cancellationToken = default
    );

    Task<File> CreateMetadataFileAsync(
        bool? ignoreDefaultVisibility,
        bool? keepRevisionForever,
        //string? ocrLanguage,
        bool? supportsAllDrives,
        // string? uploadType,
        // bool? useContentAsIndexableText,
        string? includePermissionsForView,
        string? includeLabels,
        File file,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Performs simple file upload (<c>uploadType=media</c>).
    /// </summary>
    /// <param name="ignoreDefaultVisibility"></param>
    /// <param name="keepRevisionForever"></param>
    /// <param name="ocrLanguage"></param>
    /// <param name="supportsAllDrives"></param>
    /// <param name="useContentAsIndexableText"></param>
    /// <param name="includePermissionsForView"></param>
    /// <param name="includeLabels"></param>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    Task<File> SimpleUploadFileAsync(
        bool? ignoreDefaultVisibility,
        bool? keepRevisionForever,
        string? ocrLanguage,
        bool? supportsAllDrives,
        bool? useContentAsIndexableText,
        string? includePermissionsForView,
        string? includeLabels,
        Stream stream,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Performs multipart file upload (<c>uploadType=multipart</c>).
    /// </summary>
    /// <param name="ignoreDefaultVisibility"></param>
    /// <param name="keepRevisionForever"></param>
    /// <param name="ocrLanguage"></param>
    /// <param name="supportsAllDrives"></param>
    /// <param name="useContentAsIndexableText"></param>
    /// <param name="includePermissionsForView"></param>
    /// <param name="includeLabels"></param>
    /// <param name="file"></param>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    Task<File> MultipartUploadFileAsync(
        bool? ignoreDefaultVisibility,
        bool? keepRevisionForever,
        string? ocrLanguage,
        bool? supportsAllDrives,
        bool? useContentAsIndexableText,
        string? includePermissionsForView,
        string? includeLabels,
        File file,
        Stream stream,
        CancellationToken cancellationToken = default
    );

    Task<string> InitializeResumableUploadAsync(
        bool? ignoreDefaultVisibility,
        bool? keepRevisionForever,
        string? ocrLanguage,
        bool? supportsAllDrives,
        bool? useContentAsIndexableText,
        string? includePermissionsForView,
        string? includeLabels,
        File? file,
        CancellationToken cancellationToken = default
    );
}
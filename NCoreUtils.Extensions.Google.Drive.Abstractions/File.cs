using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

/// <summary>
/// The metadata for a file.
/// </summary>
public partial class File(
    string? kind = "drive#file",
    string? id = default,
    string? name = default,
    string? mimeType = default,
    string? description = default,
    bool? starred = default,
    bool? trashed = default,
    bool? explicitlyTrashed = default,
    User? trashingUser = default,
    DateTimeOffset? trashedTime = default,
    IReadOnlyList<string>? parents = default,
    IReadOnlyDictionary<string, string>? properties = default,
    IReadOnlyDictionary<string, string>? appProperties = default,
    IReadOnlyList<string>? spaces = default,
    long? version = default,
    string? webContentLink = default,
    string? webViewLink = default,
    string? iconLink = default,
    bool? hasThumbnail = default,
    string? thumbnailLink = default,
    long? thumbnailVersion = default,
    bool? viewedByMe = default,
    DateTimeOffset? viewedByMeTime = default,
    DateTimeOffset? createdTime = default,
    DateTimeOffset? modifiedTime = default,
    DateTimeOffset? modifiedByMeTime = default,
    User? modifiedByMe = default,
    DateTimeOffset? sharedWithMeTime = default,
    User? sharingUser = default,
    IReadOnlyList<User>? owners = default,
    string? teamDriveId = default,
    string? driveId = default,
    User? lastModifyingUser = default,
    bool? shared = default,
    bool? ownedByMe = default,
    FileCapabilities? capabilities = default,
    bool? viewersCanCopyContent = default,
    bool? copyRequiresWriterPermission = default,
    bool? writersCanShare = default,
    IReadOnlyList<Permission>? permissions = default,
    IReadOnlyList<string>? permissionIds = default,
    bool? hasAugmentedPermissions = default,
    string? folderColorRgb = default,
    string? originalFilename = default,
    string? fullFileExtension = default,
    string? fileExtension = default,
    string? md5Checksum = default,
    long? size = default,
    long? quotaBytesUsed = default,
    string? headRevisionId = default,
    FileContentHints? contentHints = default,
    FileImageMediaMetadata? imageMediaMetadata = default,
    FileVideoMediaMetadata? videoMediaMetadata = default,
    bool? isAppAuthorized = default,
    IReadOnlyDictionary<string, string>? exportLinks = default,
    FileShortcutDetails? shortcutDetails = default,
    IReadOnlyList<ContentRestriction>? contentRestrictions = default,
    string? resourceKey = default,
    FileLinkShareMetadata? linkShareMetadata = default,
    FileLabelInfo? labelInfo = default,
    string? sha1Checksum = default,
    string? sha256Checksum = default,
    bool? inheritedPermissionsDisabled = default,
    DownloadRestrictionsMetadata? downloadRestrictions = default,
    ClientEncryptionDetails? clientEncryptionDetails = default)
    : IMinimalFileInfo
{
    [JsonPropertyName("kind")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Kind { get; } = kind;

    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Id { get; } = id;

    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Name { get; } = name;

    [JsonPropertyName("mimeType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MimeType { get; } = mimeType;

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; } = description;

    [JsonPropertyName("starred")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Starred { get; } = starred;

    [JsonPropertyName("trashed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Trashed { get; } = trashed;

    [JsonPropertyName("explicitlyTrashed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ExplicitlyTrashed { get; } = explicitlyTrashed;

    [JsonPropertyName("trashingUser")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public User? TrashingUser { get; } = trashingUser;

    [JsonPropertyName("trashedTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? TrashedTime { get; } = trashedTime;

    [JsonPropertyName("parents")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? Parents { get; } = parents;

    [JsonPropertyName("properties")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyDictionary<string, string>? Properties { get; } = properties;

    [JsonPropertyName("appProperties")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyDictionary<string, string>? AppProperties { get; } = appProperties;

    [JsonPropertyName("spaces")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? Spaces { get; } = spaces;

    [JsonPropertyName("version")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public long? Version { get; } = version;

    [JsonPropertyName("webContentLink")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? WebContentLink { get; } = webContentLink;

    [JsonPropertyName("webViewLink")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? WebViewLink { get; } = webViewLink;

    [JsonPropertyName("iconLink")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? IconLink { get; } = iconLink;

    [JsonPropertyName("hasThumbnail")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? HasThumbnail { get; } = hasThumbnail;

    [JsonPropertyName("thumbnailLink")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ThumbnailLink { get; } = thumbnailLink;

    [JsonPropertyName("thumbnailVersion")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public long? ThumbnailVersion { get; } = thumbnailVersion;

    [JsonPropertyName("viewedByMe")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ViewedByMe { get; } = viewedByMe;

    [JsonPropertyName("viewedByMeTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? ViewedByMeTime { get; } = viewedByMeTime;

    [JsonPropertyName("createdTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? CreatedTime { get; } = createdTime;

    [JsonPropertyName("modifiedTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? ModifiedTime { get; } = modifiedTime;

    [JsonPropertyName("modifiedByMeTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? ModifiedByMeTime { get; } = modifiedByMeTime;

    [JsonPropertyName("modifiedByMe")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public User? ModifiedByMe { get; } = modifiedByMe;

    [JsonPropertyName("sharedWithMeTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? SharedWithMeTime { get; } = sharedWithMeTime;

    [JsonPropertyName("sharingUser")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public User? SharingUser { get; } = sharingUser;

    [JsonPropertyName("owners")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<User>? Owners { get; } = owners;

    [JsonPropertyName("teamDriveId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? TeamDriveId { get; } = teamDriveId;

    [JsonPropertyName("driveId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? DriveId { get; } = driveId;

    [JsonPropertyName("lastModifyingUser")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public User? LastModifyingUser { get; } = lastModifyingUser;

    [JsonPropertyName("shared")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Shared { get; } = shared;

    [JsonPropertyName("ownedByMe")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? OwnedByMe { get; } = ownedByMe;

    [JsonPropertyName("capabilities")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public FileCapabilities? Capabilities { get; } = capabilities;

    [JsonPropertyName("viewersCanCopyContent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ViewersCanCopyContent { get; } = viewersCanCopyContent;

    [JsonPropertyName("copyRequiresWriterPermission")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CopyRequiresWriterPermission { get; } = copyRequiresWriterPermission;

    [JsonPropertyName("writersCanShare")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? WritersCanShare { get; } = writersCanShare;

    [JsonPropertyName("permissions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<Permission>? Permissions { get; } = permissions;

    [JsonPropertyName("permissionIds")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? PermissionIds { get; } = permissionIds;

    [JsonPropertyName("hasAugmentedPermissions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? HasAugmentedPermissions { get; } = hasAugmentedPermissions;

    [JsonPropertyName("folderColorRgb")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? FolderColorRgb { get; } = folderColorRgb;

    [JsonPropertyName("originalFilename")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? OriginalFilename { get; } = originalFilename;

    [JsonPropertyName("fullFileExtension")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? FullFileExtension { get; } = fullFileExtension;

    [JsonPropertyName("fileExtension")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? FileExtension { get; } = fileExtension;

    [JsonPropertyName("md5Checksum")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Md5Checksum { get; } = md5Checksum;

    [JsonPropertyName("size")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public long? Size { get; } = size;

    [JsonPropertyName("quotaBytesUsed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public long? QuotaBytesUsed { get; } = quotaBytesUsed;

    [JsonPropertyName("headRevisionId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? HeadRevisionId { get; } = headRevisionId;

    [JsonPropertyName("contentHints")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public FileContentHints? ContentHints { get; } = contentHints;

    [JsonPropertyName("imageMediaMetadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public FileImageMediaMetadata? ImageMediaMetadata { get; } = imageMediaMetadata;

    [JsonPropertyName("videoMediaMetadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public FileVideoMediaMetadata? VideoMediaMetadata { get; } = videoMediaMetadata;

    [JsonPropertyName("isAppAuthorized")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? IsAppAuthorized { get; } = isAppAuthorized;

    [JsonPropertyName("exportLinks")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyDictionary<string, string>? ExportLinks { get; } = exportLinks;

    [JsonPropertyName("shortcutDetails")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public FileShortcutDetails? ShortcutDetails { get; } = shortcutDetails;

    [JsonPropertyName("contentRestrictions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<ContentRestriction>? ContentRestrictions { get; } = contentRestrictions;

    [JsonPropertyName("resourceKey")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ResourceKey { get; } = resourceKey;

    [JsonPropertyName("linkShareMetadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public FileLinkShareMetadata? LinkShareMetadata { get; } = linkShareMetadata;

    [JsonPropertyName("labelInfo")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public FileLabelInfo? LabelInfo { get; } = labelInfo;

    [JsonPropertyName("sha1Checksum")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Sha1Checksum { get; } = sha1Checksum;

    [JsonPropertyName("sha256Checksum")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Sha256Checksum { get; } = sha256Checksum;

    [JsonPropertyName("inheritedPermissionsDisabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? InheritedPermissionsDisabled { get; } = inheritedPermissionsDisabled;

    [JsonPropertyName("downloadRestrictions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DownloadRestrictionsMetadata? DownloadRestrictions { get; } = downloadRestrictions;

    [JsonPropertyName("clientEncryptionDetails")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ClientEncryptionDetails? ClientEncryptionDetails { get; } = clientEncryptionDetails;
}
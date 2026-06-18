using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

public class PermissionDetail(
    string? permissionType,
    string? inheritedFrom,
    string? role,
    bool? inherited)
{
    [JsonPropertyName("permissionType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? PermissionType { get; } = permissionType;

    [JsonPropertyName("inheritedFrom")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? InheritedFrom { get; } = inheritedFrom;

    [JsonPropertyName("role")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Role { get; } = role;

    [JsonPropertyName("inherited")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Inherited { get; } = inherited;
}

/// <summary>
/// A permission for a file.
/// </summary>
public class Permission(
    string? kind = "drive#permission",
    string? id = default,
    string? type = default,
    string? emailAddress = default,
    string? domain = default,
    string? role = default,
    string? view = default,
    bool? allowFileDiscovery = default,
    string? displayName = default,
    IReadOnlyList<PermissionDetail>? permissionDetails = default,
    string? photoLink = default,
    DateTimeOffset? expirationTime = default,
    bool? deleted = default,
    bool? pendingOwner = default,
    bool? inheritedPermissionsDisabled = default)
{
    /// <summary>
    /// Identifies what kind of resource this is. Value: the fixed string "drive#permission".
    /// </summary>
    [JsonPropertyName("kind")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Kind { get; } = kind;

    /// <summary>
    /// The ID of this permission.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Id { get; } = id;

    /// <summary>
    /// The type of the grantee. Valid values are: user, group, domain, anyone.
    /// </summary>
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Type { get; } = type;

    /// <summary>
    /// The email address of the user or group to which this permission refers.
    /// </summary>
    [JsonPropertyName("emailAddress")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? EmailAddress { get; } = emailAddress;

    /// <summary>
    /// The domain to which this permission refers.
    /// </summary>
    [JsonPropertyName("domain")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Domain { get; } = domain;

    /// <summary>
    /// The role granted by this permission. Valid values are: owner, organizer, fileOrganizer, writer, commenter, reader.
    /// </summary>
    [JsonPropertyName("role")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Role { get; } = role;

    /// <summary>
    /// Indicates the view for this permission.
    /// </summary>
    [JsonPropertyName("view")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? View { get; } = view;

    /// <summary>
    /// Whether the permission allows the file to be discovered through search.
    /// </summary>
    [JsonPropertyName("allowFileDiscovery")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? AllowFileDiscovery { get; } = allowFileDiscovery;

    /// <summary>
    /// A displayable name for users, groups or domains.
    /// </summary>
    [JsonPropertyName("displayName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? DisplayName { get; } = displayName;

    /// <summary>
    /// Whether this permission is inherited. This field is always populated. This is an output-only field.
    /// </summary>
    [JsonPropertyName("permissionDetails")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<PermissionDetail>? PermissionDetails { get; } = permissionDetails;

    /// <summary>
    /// A link to the user's profile photo, if available.
    /// </summary>
    [JsonPropertyName("photoLink")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? PhotoLink { get; } = photoLink;

    /// <summary>
    /// The time at which this permission will expire (RFC 3339 date-time).
    /// </summary>
    [JsonPropertyName("expirationTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
    public DateTimeOffset? ExpirationTime { get; } = expirationTime;

    /// <summary>
    /// Whether the account associated with this permission has been deleted.
    /// </summary>
    [JsonPropertyName("deleted")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Deleted { get; } = deleted;

    /// <summary>
    /// Whether the grantee is the pending owner of the file.
    /// </summary>
    [JsonPropertyName("pendingOwner")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? PendingOwner { get; } = pendingOwner;

    /// <summary>
    /// When true, only organizers, owners, and users with permissions added directly on the item can access it.
    /// </summary>
    [JsonPropertyName("inheritedPermissionsDisabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? InheritedPermissionsDisabled { get; } = inheritedPermissionsDisabled;
}

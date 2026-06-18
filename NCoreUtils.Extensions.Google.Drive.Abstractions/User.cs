using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

/// <summary>
/// A user in Google Drive.
/// </summary>
public class User(
    string? kind = "drive#user",
    string? displayName = default,
    string? photoLink = default,
    bool? me = default,
    string? permissionId = default,
    string? emailAddress = default)
{
    /// <summary>
    /// Identifies what kind of resource this is. Value: the fixed string "drive#user".
    /// </summary>
    [JsonPropertyName("kind")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Kind { get; } = kind;

    /// <summary>
    /// A plain text displayable name for this user.
    /// </summary>
    [JsonPropertyName("displayName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? DisplayName { get; } = displayName;

    /// <summary>
    /// A link to the user's profile photo, if available.
    /// </summary>
    [JsonPropertyName("photoLink")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? PhotoLink { get; } = photoLink;

    /// <summary>
    /// Whether this user is the requesting user.
    /// </summary>
    [JsonPropertyName("me")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Me { get; } = me;

    /// <summary>
    /// The user's ID as visible in Permission resources.
    /// </summary>
    [JsonPropertyName("permissionId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? PermissionId { get; } = permissionId;

    /// <summary>
    /// The email address of the user. This may not be present in certain contexts if the user has not made their email address visible to the requester.
    /// </summary>
    [JsonPropertyName("emailAddress")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? EmailAddress { get; } = emailAddress;
}

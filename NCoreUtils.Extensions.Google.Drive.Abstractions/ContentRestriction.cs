using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

/// <summary>
/// A restriction for accessing the content of the file.
/// </summary>
public class ContentRestriction(
    bool? readOnly = default,
    string? reason = default,
    string? type = default,
    User? restrictingUser = default,
    DateTimeOffset? restrictionTime = default,
    bool? ownerRestricted = default,
    bool? systemRestricted = default)
{
    /// <summary>
    /// Whether the content of the file is read-only.
    /// </summary>
    [JsonPropertyName("readOnly")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ReadOnly { get; } = readOnly;

    /// <summary>
    /// Reason for why the content of the file is restricted.
    /// </summary>
    [JsonPropertyName("reason")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Reason { get; } = reason;

    /// <summary>
    /// Output only. The type of the content restriction.
    /// </summary>
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Type { get; } = type;

    /// <summary>
    /// Output only. The user who set the content restriction.
    /// </summary>
    [JsonPropertyName("restrictingUser")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public User? RestrictingUser { get; } = restrictingUser;

    /// <summary>
    /// Output only. The time at which the content restriction was set (formatted RFC 3339 timestamp).
    /// </summary>
    [JsonPropertyName("restrictionTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
    public DateTimeOffset? RestrictionTime { get; } = restrictionTime;

    /// <summary>
    /// Whether the content restriction can only be modified or removed by a user who owns the file.
    /// </summary>
    [JsonPropertyName("ownerRestricted")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? OwnerRestricted { get; } = ownerRestricted;

    /// <summary>
    /// Output only. Whether the content restriction was applied by the system.
    /// </summary>
    [JsonPropertyName("systemRestricted")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? SystemRestricted { get; } = systemRestricted;
}

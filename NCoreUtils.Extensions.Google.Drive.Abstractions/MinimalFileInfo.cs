using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

public class MinimalFileInfo(
    string? kind = "drive#file",
    string? id = default,
    string? name = default,
    string? mimeType = default,
    bool? trashed = default,
    DateTimeOffset? createdTime = default,
    DateTimeOffset? modifiedTime = default)
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

    [JsonPropertyName("trashed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Trashed { get; } = trashed;

    [JsonPropertyName("createdTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
    public DateTimeOffset? CreatedTime { get; } = createdTime;

    [JsonPropertyName("modifiedTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
    public DateTimeOffset? ModifiedTime { get; } = modifiedTime;
}

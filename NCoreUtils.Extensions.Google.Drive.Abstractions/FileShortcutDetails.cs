using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

public class FileShortcutDetails(string? targetId = default, string? targetMimeType = default, string? targetResourceKey = default)
{
    [JsonPropertyName("targetId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? TargetId { get; } = targetId;

    [JsonPropertyName("targetMimeType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? TargetMimeType { get; } = targetMimeType;

    [JsonPropertyName("targetResourceKey")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? TargetResourceKey { get; } = targetResourceKey;
}
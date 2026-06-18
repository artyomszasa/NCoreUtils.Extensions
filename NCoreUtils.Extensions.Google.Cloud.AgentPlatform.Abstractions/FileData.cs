using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class FileData(
    string mimeType,
    string fileUri,
    string? displayName)
{
    [JsonPropertyName("mimeType")]
    public string MimeType { get; } = mimeType;

    [JsonPropertyName("fileUri")]
    public string FileUri { get; } = fileUri;

    [JsonPropertyName("displayName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public string? DisplayName { get; } = displayName;
}

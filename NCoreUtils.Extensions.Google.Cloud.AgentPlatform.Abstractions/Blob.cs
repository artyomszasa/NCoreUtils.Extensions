using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class Blob(
    string mimeType,
    string data,
    string? displayName)
{
    [JsonPropertyName("mimeType")]
    public string MimeType { get; } = mimeType;

    [JsonPropertyName("data")]
    public string Data { get; } = data;

    [JsonPropertyName("displayName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public string? DisplayName { get; } = displayName;
}

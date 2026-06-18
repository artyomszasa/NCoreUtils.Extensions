using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class SafetySetting(
    string category,
    string threshold,
    string? method)
{
    [JsonPropertyName("category")]
    public string Category { get; } = category;

    [JsonPropertyName("threshold")]
    public string Threshold { get; } = threshold;

    [JsonPropertyName("method")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Method { get; } = method;
}

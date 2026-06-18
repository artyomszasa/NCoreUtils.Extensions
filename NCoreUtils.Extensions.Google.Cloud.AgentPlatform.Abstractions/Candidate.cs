using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class Candidate(
    int index,
    Content content,
    string? finishReason,
    string? finishMessage)
{
    [JsonPropertyName("index")]
    public int Index { get; } = index;

    [JsonPropertyName("content")]
    public Content Content { get; } = content;

    [JsonPropertyName("finishReason")]
    public string? FinishReason { get; } = finishReason;

    [JsonPropertyName("finishMessage")]
    public string? FinishMessage { get; } = finishMessage;
}

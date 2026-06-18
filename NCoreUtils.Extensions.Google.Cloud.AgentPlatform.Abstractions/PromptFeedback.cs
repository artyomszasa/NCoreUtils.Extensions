using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class PromptFeedback(
    string? blockReason,
    // TODO: safetyRatings
    string? blockReasonMessage)
{
    [JsonPropertyName("blockReason")]
    public string? BlockReason { get; } = blockReason;

    [JsonPropertyName("blockReasonMessage")]
    public string? BlockReasonMessage { get; } = blockReasonMessage;
}

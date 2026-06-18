using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class GenerateContentResponse(
    IReadOnlyList<Candidate> candidates,
    string? modelVersion,
    string? createTime,
    string? responseId,
    PromptFeedback? promptFeedback,
    UsageMetadata? usageMetadata)
{
    [JsonPropertyName("candidates")]
    public IReadOnlyList<Candidate> Candidates { get; } = candidates;

    [JsonPropertyName("modelVersion")]
    public string? ModelVersion { get; } = modelVersion;

    [JsonPropertyName("createTime")]
    public string? CreateTime { get; } = createTime;

    [JsonPropertyName("responseId")]
    public string? ResponseId { get; } = responseId;

    [JsonPropertyName("promptFeedback")]
    public PromptFeedback? PromptFeedback { get; } = promptFeedback;

    [JsonPropertyName("usageMetadata")]
    public UsageMetadata? UsageMetadata { get; } = usageMetadata;
}
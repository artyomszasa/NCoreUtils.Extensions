using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class UsageMetadata(
    int promptTokenCount,
    int candidatesTokenCount,
    int totalTokenCount,
    int toolUsePromptTokenCount,
    int thoughtsTokenCount,
    int cachedContentTokenCount,
    IReadOnlyList<ModalityTokenCount>? promptTokensDetails,
    IReadOnlyList<ModalityTokenCount>? cacheTokensDetails,
    IReadOnlyList<ModalityTokenCount>? candidatesTokensDetails,
    IReadOnlyList<ModalityTokenCount>? toolUsePromptTokensDetails,
    string? trafficType)
{
    [JsonPropertyName("promptTokenCount")]
    public int PromptTokenCount { get; } = promptTokenCount;

    [JsonPropertyName("candidatesTokenCount")]
    public int CandidatesTokenCount { get; } = candidatesTokenCount;

    [JsonPropertyName("totalTokenCount")]
    public int TotalTokenCount { get; } = totalTokenCount;

    [JsonPropertyName("toolUsePromptTokenCount")]
    public int ToolUsePromptTokenCount { get; } = toolUsePromptTokenCount;

    [JsonPropertyName("thoughtsTokenCount")]
    public int ThoughtsTokenCount { get; } = thoughtsTokenCount;

    [JsonPropertyName("cachedContentTokenCount")]
    public int CachedContentTokenCount { get; } = cachedContentTokenCount;

    [JsonPropertyName("promptTokensDetails")]
    public IReadOnlyList<ModalityTokenCount>? PromptTokensDetails { get; } = promptTokensDetails;

    [JsonPropertyName("cacheTokensDetails")]
    public IReadOnlyList<ModalityTokenCount>? CacheTokensDetails { get; } = cacheTokensDetails;

    [JsonPropertyName("candidatesTokensDetails")]
    public IReadOnlyList<ModalityTokenCount>? CandidatesTokensDetails { get; } = candidatesTokensDetails;

    [JsonPropertyName("toolUsePromptTokensDetails")]
    public IReadOnlyList<ModalityTokenCount>? ToolUsePromptTokensDetails { get; } = toolUsePromptTokensDetails;

    [JsonPropertyName("trafficType")]
    public string? TrafficType { get; } = trafficType;
}

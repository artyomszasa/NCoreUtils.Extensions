using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

[method: JsonConstructor]
public readonly struct ModalityTokenCount(string? modality, int tokenCount)
{
    [JsonPropertyName("modality")]
    public string? Modality { get; } = modality;

    [JsonPropertyName("tokenCount")]
    public int TokenCount { get; } = tokenCount;

    public override string ToString()
        => $"{TokenCount} {Modality}";
}

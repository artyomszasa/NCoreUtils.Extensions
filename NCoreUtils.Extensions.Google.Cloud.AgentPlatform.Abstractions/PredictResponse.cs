using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class PredictResponse(
    IReadOnlyList<JsonElement> predictions,
    string? deployedModelId,
    string? model,
    string? modelVersionId,
    string? modelDisplayName,
    JsonElement? metadata)
{
    [JsonPropertyName("predictions")]
    public IReadOnlyList<JsonElement> Predictions { get; } = predictions;

    [JsonPropertyName("deployedModelId")]
    public string? DeployedModelId { get; } = deployedModelId;

    [JsonPropertyName("model")]
    public string? Model { get; } = model;

    [JsonPropertyName("modelVersionId")]
    public string? ModelVersionId { get; } = modelVersionId;

    [JsonPropertyName("modelDisplayName")]
    public string? ModelDisplayName { get; } = modelDisplayName;

    [JsonPropertyName("metadata")]
    public JsonElement? Metadata { get; } = metadata;
}
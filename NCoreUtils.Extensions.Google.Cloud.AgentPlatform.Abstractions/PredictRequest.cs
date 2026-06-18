using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class PredictRequest(string endpoint, IReadOnlyList<PredictRequestInstance> instances, PredictRequestParameters parameters)
{
    [JsonPropertyName("endpoint")]
    public string Endpoint { get; } = endpoint;

    [JsonPropertyName("instances")]
    public IReadOnlyList<PredictRequestInstance> Instances { get; } = instances;

    [JsonPropertyName("parameters")]
    public PredictRequestParameters Parameters { get; } = parameters;
}

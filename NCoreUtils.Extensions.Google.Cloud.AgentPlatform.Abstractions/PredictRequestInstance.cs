using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class PredictRequestInstance(string prompt)
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; } = prompt;
}

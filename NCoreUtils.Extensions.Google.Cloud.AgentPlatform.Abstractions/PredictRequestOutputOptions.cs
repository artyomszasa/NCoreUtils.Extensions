using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class PredictRequestOutputOptions(string? mimeType, int? compressionQuality)
{
    [JsonPropertyName("mimeType")]
    public string? MimeType { get; } = mimeType;

    [JsonPropertyName("compressionQuality")]
    public int? CompressionQuality { get; } = compressionQuality;
}

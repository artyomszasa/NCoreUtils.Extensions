using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.Monitoring;

public class Metric(
    string type,
    IReadOnlyDictionary<string, string>? labels = default)
{
    [JsonPropertyName("type")]
    public string Type { get; } = type;

    [JsonPropertyName("labels")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyDictionary<string, string>? Labels { get; } = labels;
}

using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class DetectLanguageRequest(
    string content,
    string? mimeType = default,
    string? model = default,
    IReadOnlyDictionary<string, string>? labels = default)
{
    [JsonPropertyName("content")]
    public string Content { get; } = content;

    [JsonPropertyName("mimeType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MimeType { get; } = mimeType;

    [JsonPropertyName("model")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Model { get; } = model;

    [JsonPropertyName("labels")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyDictionary<string, string>? Labels { get; } = labels;
}

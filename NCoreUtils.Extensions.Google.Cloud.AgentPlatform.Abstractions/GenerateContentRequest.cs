using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

[method: JsonConstructor]
public class GenerateContentRequest(
    IReadOnlyList<Content> contents,
    string? cachedContent = default,
    // TODO: tools
    // TODO: toolConfig
    IReadOnlyList<SafetySetting>? safetySettings = default,
    // TODO: modelArmorConfig
    GenerationConfig? generationConfig = default,
    Content? systemInstruction = default,
    IReadOnlyDictionary<string, string>? labels = default)
{
    public static implicit operator GenerateContentRequest(string text) => new(text);

    [JsonPropertyName("contents")]
    public IReadOnlyList<Content> Contents { get; } = contents;

    [JsonPropertyName("cachedContent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? CachedContent { get; } = cachedContent;

    [JsonPropertyName("safetySettings")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<SafetySetting>? SafetySettings { get; } = safetySettings;

    [JsonPropertyName("generationConfig")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public GenerationConfig? GenerationConfig { get; } = generationConfig;

    [JsonPropertyName("systemInstruction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Content? SystemInstruction { get; } = systemInstruction;

    [JsonPropertyName("labels")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyDictionary<string, string>? Labels { get; } = labels;

    public GenerateContentRequest(
        Content content,
        string? cachedContent = default,
        IReadOnlyList<SafetySetting>? safetySettings = default,
        GenerationConfig? generationConfig = default,
        Content? systemInstruction = default,
        IReadOnlyDictionary<string, string>? labels = default)
        : this([content], cachedContent, safetySettings, generationConfig, systemInstruction, labels)
    { }
}
using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

[method: JsonConstructor]
public class TranslateTextRequest(
    IReadOnlyList<string> contents,
    string? mimeType = default,
    string? sourceLanguageCode = default,
    string? targetLanguageCode = default,
    string? model = default,
    TranslateTextGlossaryConfig? glossaryConfig = default,
    TransliterationConfig? transliterationConfig = default,
    IReadOnlyDictionary<string, string>? labels = default)
{
    [JsonPropertyName("contents")]
    public IReadOnlyList<string> Contents { get; } = contents;

    [JsonPropertyName("mimeType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MimeType { get; } = mimeType;

    [JsonPropertyName("sourceLanguageCode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? SourceLanguageCode { get; } = sourceLanguageCode;

    [JsonPropertyName("targetLanguageCode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? TargetLanguageCode { get; } = targetLanguageCode;

    [JsonPropertyName("model")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Model { get; } = model;

    [JsonPropertyName("glossaryConfig")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public TranslateTextGlossaryConfig? GlossaryConfig { get; } = glossaryConfig;

    [JsonPropertyName("transliterationConfig")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public TransliterationConfig? TransliterationConfig { get; } = transliterationConfig;

    [JsonPropertyName("labels")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyDictionary<string, string>? Labels { get; } = labels;

    public TranslateTextRequest(
        string content,
        string? mimeType = default,
        string? sourceLanguageCode = default,
        string? targetLanguageCode = default,
        string? model = default,
        TranslateTextGlossaryConfig? glossaryConfig = default,
        TransliterationConfig? transliterationConfig = default,
        IReadOnlyDictionary<string, string>? labels = default)
        : this([content], mimeType, sourceLanguageCode, targetLanguageCode, model, glossaryConfig, transliterationConfig, labels)
    { }
}

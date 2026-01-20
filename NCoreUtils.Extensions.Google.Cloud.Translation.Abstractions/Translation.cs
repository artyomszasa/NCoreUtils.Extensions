using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class Translation(
    string translatedText,
    string? model,
    string? detectedLanguageCode,
    TranslateTextGlossaryConfig? glossaryConfig)
{
    [JsonPropertyName("translatedText")]
    public string TranslatedText { get; } = translatedText;

    [JsonPropertyName("model")]
    public string? Model { get; } = model;

    [JsonPropertyName("detectedLanguageCode")]
    public string? DetectedLanguageCode { get; } = detectedLanguageCode;

    [JsonPropertyName("glossaryConfig")]
    public TranslateTextGlossaryConfig? GlossaryConfig { get; } = glossaryConfig;
}

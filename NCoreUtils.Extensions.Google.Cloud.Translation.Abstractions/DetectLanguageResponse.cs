using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class DetectLanguageResponse(IReadOnlyList<DetectedLanguage> languages)
{
    [JsonPropertyName("languages")]
    public IReadOnlyList<DetectedLanguage> Languages { get; } = languages;
}

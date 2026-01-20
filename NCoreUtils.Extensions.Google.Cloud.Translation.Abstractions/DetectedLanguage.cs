using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class DetectedLanguage(
    string languageCode,
    double confidence)
{
    [JsonPropertyName("languageCode")]
    public string LanguageCode { get; } = languageCode;

    [JsonPropertyName("confidence")]
    public double Confidence { get; } = confidence;
}

using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class SupportedLanguage(
    string languageCode,
    string displayName,
    bool supportSource,
    bool supportTarget)
{
    [JsonPropertyName("languageCode")]
    public string LanguageCode { get; } = languageCode;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; } = displayName;

    [JsonPropertyName("supportSource")]
    public bool SupportSource { get; } = supportSource;

    [JsonPropertyName("supportTarget")]
    public bool SupportTarget { get; } = supportTarget;
}

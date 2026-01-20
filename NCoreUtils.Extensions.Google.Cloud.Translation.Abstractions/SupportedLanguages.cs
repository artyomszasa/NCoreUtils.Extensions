using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class SupportedLanguages(IReadOnlyList<SupportedLanguage> languages)
{
    [JsonPropertyName("languages")]
    public IReadOnlyList<SupportedLanguage> Languages { get; } = languages;
}

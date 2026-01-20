using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class TransliterationConfig(bool enableTransliteration)
{
    [JsonPropertyName("enableTransliteration")]
    public bool EnableTransliteration { get; } = enableTransliteration;
}

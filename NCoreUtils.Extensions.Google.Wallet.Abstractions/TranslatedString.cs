using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class TranslatedString(string language, string value)
{
    [JsonPropertyName("language")]
    public string Language { get; } = language;

    [JsonPropertyName("value")]
    public string Value { get; } = value;
}

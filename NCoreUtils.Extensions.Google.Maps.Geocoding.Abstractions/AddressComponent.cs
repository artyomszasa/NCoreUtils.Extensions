using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Maps.Geocoding;

public class AddressComponent(
    string? longText,
    string? shortText,
    IReadOnlyList<string> types,
    string? languageCode)
{
    [JsonPropertyName("longText")]
    public string? LongText { get; } = longText;

    [JsonPropertyName("shortText")]
    public string? ShortText { get; } = shortText;

    [JsonPropertyName("types")]
    public IReadOnlyList<string> Types { get; } = types;

    [JsonPropertyName("languageCode")]
    public string? LanguageCode { get; } = languageCode;
}


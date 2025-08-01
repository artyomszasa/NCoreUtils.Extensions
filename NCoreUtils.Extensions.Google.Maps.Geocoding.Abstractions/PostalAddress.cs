using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Maps.Geocoding;

public class PostalAddress(
    long revision = default,
    string? regionCode = default,
    string? languageCode = default,
    string? postalCode = default,
    string? sortingCode = default,
    string? administrativeArea = default,
    string? locality = default,
    string? sublocality = default,
    IReadOnlyList<string>? addressLines = default,
    IReadOnlyList<string>? recipients = default,
    string? organization = default)
{
    [JsonPropertyName("revision")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public long Revision { get; } = revision;

    [JsonPropertyName("regionCode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? RegionCode { get; } = regionCode;

    [JsonPropertyName("languageCode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? LanguageCode { get; } = languageCode;

    [JsonPropertyName("postalCode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? PostalCode { get; } = postalCode;

    [JsonPropertyName("sortingCode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? SortingCode { get; } = sortingCode;

    [JsonPropertyName("administrativeArea")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AdministrativeArea { get; } = administrativeArea;

    [JsonPropertyName("locality")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Locality { get; } = locality;

    [JsonPropertyName("sublocality")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Sublocality { get; } = sublocality;

    [JsonPropertyName("addressLines")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? AddressLines { get; } = addressLines;

    [JsonPropertyName("recipients")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? Recipients { get; } = recipients;

    [JsonPropertyName("organization")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Organization { get; } = organization;
}


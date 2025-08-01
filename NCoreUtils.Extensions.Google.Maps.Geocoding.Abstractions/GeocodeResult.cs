using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Maps.Geocoding;

public class GeocodeResult(
    string? place,
    string? placeId,
    LatLng location,
    string granularity,
    Viewport viewport,
    Viewport? bounds,
    string formattedAddress,
    PostalAddress? postalAddress,
    IReadOnlyList<AddressComponent> addressComponents,
    IReadOnlyList<string> types,
    PlusCode? plusCode)
{
    [JsonPropertyName("place")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Place { get; } = place;

    [JsonPropertyName("placeId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? PlaceId { get; } = placeId;

    [JsonPropertyName("location")]
    public LatLng Location { get; } = location;

    [JsonPropertyName("granularity")]
    public string Granularity { get; } = granularity;

    [JsonPropertyName("viewport")]
    public Viewport Viewport { get; } = viewport;

    [JsonPropertyName("bounds")]
    public Viewport? Bounds { get; } = bounds;

    [JsonPropertyName("formattedAddress")]
    public string FormattedAddress { get; } = formattedAddress;

    [JsonPropertyName("postalAddress")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public PostalAddress? PostalAddress { get; } = postalAddress;

    [JsonPropertyName("addressComponents")]
    public IReadOnlyList<AddressComponent> AddressComponents { get; } = addressComponents;

    // TODO: postalCodeLocalities

    [JsonPropertyName("types")]
    public IReadOnlyList<string> Types { get; } = types;

    [JsonPropertyName("plusCode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public PlusCode? PlusCode { get; } = plusCode;

    public override string ToString()
        => $"{Location} {FormattedAddress}";
}


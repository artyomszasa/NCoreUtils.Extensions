using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Maps.Geocoding;

[method: JsonConstructor]
public readonly struct GeocodeAddressResponse(IReadOnlyList<GeocodeResult>? results)
{
    [JsonPropertyName("results")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<GeocodeResult>? Results { get; } = results;
}


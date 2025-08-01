using NCoreUtils.Google.Maps.Geocoding;

namespace NCoreUtils.Google;

public class GeocodingClient(IGeocodingV4BetaApi api) : IGeocodingClient
{
    public IGeocodingV4BetaApi Api { get; } = api ?? throw new ArgumentNullException(nameof(api));

    public Task<GeocodeAddressResponse> AddressAsync(
        string addressString,
        string? languageCode = null,
        string? regionCode = null,
        CancellationToken cancellationToken = default)
        => Api.AddressAsync(addressString, languageCode, regionCode, cancellationToken);

    public Task<GeocodeAddressResponse> AddressAsync(
        PostalAddress postalAddress,
        CancellationToken cancellationToken = default)
        => Api.StructuredAddressAsync(postalAddress, cancellationToken);
}
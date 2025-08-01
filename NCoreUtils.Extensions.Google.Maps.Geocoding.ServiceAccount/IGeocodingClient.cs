using NCoreUtils.Google.Maps.Geocoding;

namespace NCoreUtils;

public interface IGeocodingClient
{
    Task<GeocodeAddressResponse> AddressAsync(
        string addressString,
        string? languageCode = default,
        string? regionCode = default,
        CancellationToken cancellationToken = default
    );

    Task<GeocodeAddressResponse> AddressAsync(
        PostalAddress postalAddress,
        CancellationToken cancellationToken = default
    );
}
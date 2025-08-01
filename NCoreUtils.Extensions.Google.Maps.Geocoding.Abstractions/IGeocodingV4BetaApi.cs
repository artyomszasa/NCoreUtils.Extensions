namespace NCoreUtils.Google.Maps.Geocoding;

public interface IGeocodingV4BetaApi
{
    Task<GeocodeAddressResponse> AddressAsync(
        string addressString,
        string? languageCode = default,
        string? regionCode = default,
        CancellationToken cancellationToken = default
    );

    Task<GeocodeAddressResponse> StructuredAddressAsync(
        PostalAddress postalAddress,
        CancellationToken cancellationToken = default
    );
}


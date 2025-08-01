using System.Buffers;
using System.Text.Json.Serialization;
using NCoreUtils.Google.Maps.Geocoding.Proto;
using NCoreUtils.Proto;

namespace NCoreUtils.Google.Maps.Geocoding;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(JsonRootGeocodingV4BetaApiInfo))]
internal partial class GeocodingV4BetaApiSerializerContext : JsonSerializerContext { }

[ProtoClient(typeof(GeocodingV4BetaApiInfo), typeof(GeocodingV4BetaApiSerializerContext))]
public partial class GeocodingV4BetaApiClient
{
    public const string HttpClientConfigurationName = nameof(GeocodingV4BetaApiClient);

    private static string CreateAddressRequestUri(string pathBase, string addressString, string? languageCode, string? regionCode)
    {
        var buffer = ArrayPool<char>.Shared.Rent(4096);
        try
        {
            var builder = new SpanBuilder(buffer);
            builder.Append(pathBase);
            builder.Append('/');
            builder.AppendUriEscaped(addressString);
            var fst = true;
            builder.AppendQueryParameter("languageCode", languageCode, ref fst);
            builder.AppendQueryParameter("regionCode", regionCode, ref fst);
            return builder.ToString();
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private static string CreateAddressRequestUri(string pathBase, PostalAddress postalAddress)
    {
        var buffer = ArrayPool<char>.Shared.Rent(4096);
        try
        {
            var builder = new SpanBuilder(buffer);
            builder.Append(pathBase);
            builder.Append('/');
            var fst = true;
            builder.AppendQueryParameter("languageCode", postalAddress.LanguageCode, ref fst);
            builder.AppendQueryParameter("regionCode", postalAddress.RegionCode, ref fst);
            builder.AppendQueryParameter("address.administrativeArea", postalAddress.AdministrativeArea, ref fst);
            builder.AppendQueryParameter("address.locality", postalAddress.Locality, ref fst);
            builder.AppendQueryParameter("address.sublocality", postalAddress.Sublocality, ref fst);
            builder.AppendQueryParameter("address.addressLines", postalAddress.AddressLines, ref fst);
            return builder.ToString();
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private HttpRequestMessage CreateAddressRequest(string addressString, string? languageCode, string? regionCode)
    {
        if (addressString is null)
        {
            throw new ArgumentNullException(nameof(addressString));
        }
        var pathBase = GetCachedMethodPath(Methods.Address);
        var path = CreateAddressRequestUri(pathBase, addressString, languageCode, regionCode);
        var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.SetRequiredGcpScope("https://www.googleapis.com/auth/maps-platform.geocode");
        return request;
    }

    private HttpRequestMessage CreateStructuredAddressRequest(PostalAddress postalAddress)
    {
        if (postalAddress is null)
        {
            throw new ArgumentNullException(nameof(postalAddress));
        }
        var pathBase = GetCachedMethodPath(Methods.Address);
        var path = CreateAddressRequestUri(pathBase, postalAddress);
        var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.SetRequiredGcpScope("https://www.googleapis.com/auth/maps-platform.geocode");
        return request;
    }

    protected override ValueTask HandleErrors(HttpResponseMessage response, CancellationToken cancellationToken)
        => response.HandleGoogleCloudErrorResponseAsync(cancellationToken);
}
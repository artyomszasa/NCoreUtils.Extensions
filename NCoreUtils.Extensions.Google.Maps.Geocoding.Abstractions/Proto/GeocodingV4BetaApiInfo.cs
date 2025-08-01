using NCoreUtils.Proto;

namespace NCoreUtils.Google.Maps.Geocoding.Proto;

[ProtoInfo(typeof(IGeocodingV4BetaApi), Path = "v4beta/geocode")]
[ProtoMethodInfo(nameof(IGeocodingV4BetaApi.AddressAsync), Path = "address", Input = InputType.Custom)]
[ProtoMethodInfo(nameof(IGeocodingV4BetaApi.StructuredAddressAsync), Path = "address", Input = InputType.Custom)]
public partial class GeocodingV4BetaApiInfo { }
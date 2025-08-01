using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NCoreUtils;
using NCoreUtils.Google.Maps.Geocoding;

await using var serviceProvider = new ServiceCollection()
    .AddLogging(b => b.ClearProviders().AddSimpleConsole(o => o.SingleLine = true))
    .AddGeocodingClient()
    .BuildServiceProvider(validateScopes: true);

var client = serviceProvider.GetRequiredService<IGeocodingClient>();
// var res = await client.AddressAsync("Budapest, Szabadság tér 1", languageCode: "hu");

var res = await client.AddressAsync(new PostalAddress(
    languageCode: "hu",
    locality: "Budapest",
    addressLines: ["Szabadság tér 1"]
));

Console.WriteLine(res.Results);
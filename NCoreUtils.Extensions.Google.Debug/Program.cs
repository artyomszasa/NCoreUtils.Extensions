using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NCoreUtils;
using NCoreUtils.Google.Maps.Geocoding;

await using var serviceProvider = new ServiceCollection()
    .AddLogging(b => b.ClearProviders().AddSimpleConsole(o => o.SingleLine = true))
    .AddGeocodingClient()
    .AddGoogleTranslationClient(
        projectId: Environment.GetEnvironmentVariable("PROJECT_ID")!
        // location: Environment.GetEnvironmentVariable("LOCATION")!
    )
    .BuildServiceProvider(validateScopes: true);

// await DebugGeocoding(serviceProvider);
await DebugTranslation(serviceProvider);


static async Task DebugTranslation(IServiceProvider serviceProvider)
{
    var client = serviceProvider.GetRequiredService<IGoogleTranslationClient>();

    var supportedLanguages = await client.GetSupportedLanguagesAsync();
    foreach (var language in supportedLanguages.Languages)
    {
        Console.WriteLine($"{language.LanguageCode} {language.DisplayName} => [S = {language.SupportSource}, T = {language.SupportTarget}]");
    }

    var textHu = "Fiatal felnőtt hölgy kezében tartja egy díszes üveg bort.";
    var detect1 = await client.DetectLanguageAsync(textHu, mimeType: "text/plain");
    var detect2 = await client.DetectLanguageAsync("Youn adult lady holding a fancy bottle of wine.", mimeType: "text/plain");

    var translate0 = await client.TranslateTextAsync(
        content: textHu,
        mimeType: "text/plain",
        sourceLanguageCode: detect1.Languages.MaxBy(e => e.Confidence)?.LanguageCode,
        targetLanguageCode: "en"
    );

    foreach (var translation in translate0.Translations)
    {
        Console.WriteLine(translation.TranslatedText);
    }

    var translate1 = await client.TranslateTextAsync(
        content: textHu,
        mimeType: "text/plain",
        sourceLanguageCode: detect1.Languages.MaxBy(e => e.Confidence)?.LanguageCode,
        targetLanguageCode: "en",
        model: "projects/hosting-666/locations/us-central1/models/general/translation-llm"
    );

    foreach (var translation in translate1.Translations)
    {
        Console.WriteLine(translation.TranslatedText);
    }
}

static async Task DebugGeocoding(IServiceProvider serviceProvider)
{
    var client = serviceProvider.GetRequiredService<IGeocodingClient>();
    // var res = await client.AddressAsync("Budapest, Szabadság tér 1", languageCode: "hu");

    var res = await client.AddressAsync(new PostalAddress(
        languageCode: "hu",
        locality: "Budapest",
        addressLines: ["Szabadság tér 1"]
    ));

    Console.WriteLine(res.Results);
}
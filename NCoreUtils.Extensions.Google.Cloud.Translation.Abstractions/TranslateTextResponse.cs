using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class TranslateTextResponse(
    IReadOnlyList<Translation> translations,
    IReadOnlyList<Translation>? glossaryTranslations)
{
    [JsonPropertyName("translations")]
    public IReadOnlyList<Translation> Translations { get; } = translations;

    [JsonPropertyName("glossaryTranslations")]
    public IReadOnlyList<Translation>? GlossaryTranslations { get; } = glossaryTranslations;
}
using NCoreUtils.Google;

namespace NCoreUtils;

public interface IGoogleTranslationClient
{
    Task<DetectLanguageResponse> DetectLanguageAsync(
        string content,
        string? mimeType = default,
        string? model = default,
        IReadOnlyDictionary<string, string>? labels = default,
        CancellationToken cancellationToken = default
    );

    Task<SupportedLanguages> GetSupportedLanguagesAsync(CancellationToken cancellationToken = default);

    Task<TranslateTextResponse> TranslateTextAsync(
        IReadOnlyList<string> contents,
        string? mimeType = default,
        string? sourceLanguageCode = default,
        string? targetLanguageCode = default,
        string? model = default,
        TranslateTextGlossaryConfig? glossaryConfig = default,
        TransliterationConfig? transliterationConfig = default,
        IReadOnlyDictionary<string, string>? labels = default,
        CancellationToken cancellationToken = default
    );

    #region extensions

    Task<TranslateTextResponse> TranslateTextAsync(
        string content,
        string? mimeType = default,
        string? sourceLanguageCode = default,
        string? targetLanguageCode = default,
        string? model = default,
        TranslateTextGlossaryConfig? glossaryConfig = default,
        TransliterationConfig? transliterationConfig = default,
        IReadOnlyDictionary<string, string>? labels = default,
        CancellationToken cancellationToken = default)
        => TranslateTextAsync(
            [content],
            mimeType,
            sourceLanguageCode,
            targetLanguageCode,
            model,
            glossaryConfig,
            transliterationConfig,
            labels,
            cancellationToken
        );

    #endregion
}
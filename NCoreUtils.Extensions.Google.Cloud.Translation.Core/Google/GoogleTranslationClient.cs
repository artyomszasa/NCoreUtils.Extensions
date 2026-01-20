
namespace NCoreUtils.Google;

public class GoogleTranslationClient(ITranslationApiV3 api, string projectId, string? location)
    : IGoogleTranslationClient
{
    public Task<DetectLanguageResponse> DetectLanguageAsync(
        string content,
        string? mimeType = null,
        string? model = null,
        IReadOnlyDictionary<string, string>? labels = null,
        CancellationToken cancellationToken = default)
        => api.DetectLanguageAsync(
            projectId,
            location,
            new(content, mimeType, model, labels),
            cancellationToken
        );

    public Task<SupportedLanguages> GetSupportedLanguagesAsync(CancellationToken cancellationToken = default)
        => api.GetSupportedLanguagesAsync(projectId, location, cancellationToken);

    public Task<TranslateTextResponse> TranslateTextAsync(
        IReadOnlyList<string> contents,
        string? mimeType = null,
        string? sourceLanguageCode = null,
        string? targetLanguageCode = null,
        string? model = null,
        TranslateTextGlossaryConfig? glossaryConfig = null,
        TransliterationConfig? transliterationConfig = null,
        IReadOnlyDictionary<string, string>? labels = null,
        CancellationToken cancellationToken = default)
        => api.TranslateTextAsync(
            projectId,
            location,
            new(contents, mimeType, sourceLanguageCode, targetLanguageCode, model, glossaryConfig, transliterationConfig, labels),
            cancellationToken
        );
}
namespace NCoreUtils.Google;

public interface ITranslationApiV3
{
    Task<DetectLanguageResponse> DetectLanguageAsync(
        string projectId,
        string? location,
        DetectLanguageRequest request,
        CancellationToken cancellationToken = default
    );

    Task<SupportedLanguages> GetSupportedLanguagesAsync(
        string projectId,
        string? location,
        CancellationToken cancellationToken = default
    );

    Task<TranslateTextResponse> TranslateTextAsync(
        string projectId,
        string? location,
        TranslateTextRequest request,
        CancellationToken cancellationToken = default
    );
}
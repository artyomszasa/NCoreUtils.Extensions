using System.Text.Json.Serialization;
using NCoreUtils.Google.Proto;
using NCoreUtils.Proto;
using NCoreUtils.Proto.Internal;
using HttpMethod = System.Net.Http.HttpMethod;

namespace NCoreUtils.Google;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(JsonRootTranslationApiV3Info))]
[JsonSerializable(typeof(DetectLanguageRequest))]
[JsonSerializable(typeof(TranslateTextRequest))]
internal partial class TranslationApiV3SerializerContext : JsonSerializerContext { }

[ProtoClient(typeof(TranslationApiV3Info), typeof(TranslationApiV3SerializerContext))]
public partial class TranslationApiV3Client
{
    public const string HttpClientConfigurationName = nameof(TranslationApiV3Client);

    public const string CloudTranslationScope = "https://www.googleapis.com/auth/cloud-translation";

    private HttpRequestMessage CreateDetectLanguageRequest(string projectId, string? location, DetectLanguageRequest request)
    {
        var basePath = GetCachedMethodPath(Methods.DetectLanguage);
        var path = string.IsNullOrEmpty(location)
            ? $"{basePath}/{projectId}:detectLanguage"
            : $"{basePath}/{projectId}/locations/{location}:detectLanguage";
        var req = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = ProtoJsonContent.Create(request, TranslationApiV3SerializerContext.Default.DetectLanguageRequest)
        };
        req.SetRequiredGcpScope(CloudTranslationScope);
        return req;
    }

    private HttpRequestMessage CreateGetSupportedLanguagesRequest(string projectId, string? location)
    {
        var basePath = GetCachedMethodPath(Methods.DetectLanguage);
        var path = $"{basePath}/{projectId}/supportedLanguages";
        var req = new HttpRequestMessage(HttpMethod.Get, path);
        req.SetRequiredGcpScope(CloudTranslationScope);
        return req;
    }

    private HttpRequestMessage CreateTranslateTextRequest(string projectId, string? location, TranslateTextRequest request)
    {
        var basePath = GetCachedMethodPath(Methods.DetectLanguage);
        var path = string.IsNullOrEmpty(location)
            ? $"{basePath}/{projectId}:translateText"
            : $"{basePath}/{projectId}/locations/{location}:translateText";
        var req = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = ProtoJsonContent.Create(request, TranslationApiV3SerializerContext.Default.TranslateTextRequest)
        };
        req.SetRequiredGcpScope(CloudTranslationScope);
        return req;
    }

    protected override ValueTask HandleErrors(HttpResponseMessage response, CancellationToken cancellationToken)
        => response.HandleGoogleCloudErrorResponseAsync(cancellationToken);
}
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(GoogleErrorResponse))]
internal partial class GoogleCloudApiErrorSerializerContext : JsonSerializerContext { }

#if NETSTANDARD2_1 || NETFRAMEWORK

internal static class HttpContentCompatExtensions
{
    public static Task<string> ReadAsStringAsync(this HttpContent content, CancellationToken cancellationToken)
        => content.ReadAsStringAsync();
}

#endif

public static class HttpResponseMessageExtensions
{
    public static async ValueTask HandleGoogleCloudErrorResponseAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = response.Content is null
                ? null
                : await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (responseContent is null or "")
            {
                throw new GoogleCloudException($"Server responded with status code {response.StatusCode} (no body).");
            }
            GoogleErrorResponse? gresponse;
            try
            {
                gresponse = JsonSerializer.Deserialize(responseContent, GoogleCloudApiErrorSerializerContext.Default.GoogleErrorResponse);
            }
            catch
            {
                throw new GoogleCloudException($"Server responded with status code {response.StatusCode} and unrecognized body: {responseContent}.");
            }
            throw new GoogleCloudException(gresponse?.Error);
        }
    }
}
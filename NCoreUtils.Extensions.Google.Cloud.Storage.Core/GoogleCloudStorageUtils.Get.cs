using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using NCoreUtils.Google;

namespace NCoreUtils;

public partial class GoogleCloudStorageUtils
{
    public async Task<GoogleObjectData?> GetAsync(
        string bucket,
        string name,
        string? accessToken = default,
        CancellationToken cancellationToken = default)
    {
        var requestUri = EndpointFactory.Get(bucket, name);
        using var client = CreateHttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        request.SetRequiredGcpScope(ReadOnlyScope);
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
        await HandleErrors(response).ConfigureAwait(false);
        return await response
            .Content
            .ReadFromJsonAsync(GoogleJsonContext.Default.GoogleObjectData, cancellationToken)
            .ConfigureAwait(false);
    }
}
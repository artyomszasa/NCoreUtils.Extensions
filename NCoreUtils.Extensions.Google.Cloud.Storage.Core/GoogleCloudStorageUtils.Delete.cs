using System.Net.Http.Headers;
using NCoreUtils.Google;

namespace NCoreUtils;

public partial class GoogleCloudStorageUtils
{
    public async Task DeleteAsync(
        string bucket,
        string name,
        string? accessToken = default,
        CancellationToken cancellationToken = default)
    {
        var requestUri = EndpointFactory.Delete(bucket, name);
        using var client = CreateHttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        request.SetRequiredGcpScope(ReadWriteScope);
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }
}
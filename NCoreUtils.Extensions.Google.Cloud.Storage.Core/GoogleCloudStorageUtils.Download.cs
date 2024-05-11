using System.Net.Http.Headers;
using NCoreUtils.Google;

namespace NCoreUtils;

public partial class GoogleCloudStorageUtils
{
    public async Task DownloadAsync(
        string bucket,
        string name,
        Stream destination,
        string? accessToken = default,
        CancellationToken cancellationToken = default)
    {
        var requestUri = EndpointFactory.Download(bucket, name);
        using var client = CreateHttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        request.SetRequiredGcpScope(ReadOnlyScope);
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);
        await response
            .EnsureSuccessStatusCode()
            .Content
            .CopyToAsync(destination, cancellationToken)
            .ConfigureAwait(false);
        await destination.FlushAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
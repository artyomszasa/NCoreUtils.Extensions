using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using NCoreUtils.Google;

namespace NCoreUtils;

public partial class GoogleCloudStorageUtils
{
    private async IAsyncEnumerable<GoogleObjectsPage> ListAsync(
        string bucket,
        string? prefix,
        bool? includeAcl,
        string? accessToken,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var client = CreateHttpClient();
        string? pageToken = null;
        do
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                EndpointFactory.List(bucket, prefix, includeAcl, pageToken)
            );
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            request.SetRequiredGcpScope(ReadOnlyScope);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);
            await HandleErrors(response).ConfigureAwait(false);
            var page = await response.Content
                .ReadFromJsonAsync(GoogleJsonContext.Default.GoogleObjectsData, cancellationToken)
                .ConfigureAwait(false);
            if (page is null)
            {
                pageToken = default;
            }
            else
            {
                yield return new GoogleObjectsPage(page.Prefixes, page.Items);
                pageToken = page.NextPageToken;
            }
        }
        while (pageToken is not null);
    }

    public IAsyncEnumerable<GoogleObjectsPage> ListAsync(string bucket, string? prefix, bool? includeAcl = false, string? accessToken = default)
        => new DelayedAsyncEnumerable<GoogleObjectsPage>(cancellationToken =>
        {
            var enumerable = ListAsync(
                bucket,
                prefix,
                includeAcl,
                accessToken,
                cancellationToken
            );
            return new ValueTask<IAsyncEnumerable<GoogleObjectsPage>>(enumerable);
        });
}
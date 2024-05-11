using System.Net.Http.Headers;
using System.Net.Http.Json;
using NCoreUtils.Google;

namespace NCoreUtils;

public partial class GoogleCloudStorageUtils
{
    public async Task<GoogleObjectData> PatchAsync(
        string bucket,
        string name,
        GoogleObjectPatchData patch,
        bool? includeAcl = false,
        string? accessToken = default,
        CancellationToken cancellationToken = default)
    {
        if (patch is null)
        {
            throw new ArgumentNullException(nameof(patch));
        }
        var requestUri = EndpointFactory.Patch(bucket, name, includeAcl);
        using var client = CreateHttpClient();
        using var request = new HttpRequestMessage(
            HttpMethod.Patch,
            requestUri
        );
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        // request.Content = JsonContent.Create(patch, _jsonMediaType, GoogleJsonContext.Default.Options);
        request.Content = new JsonContent<GoogleObjectPatchData>(patch, GoogleJsonContext.Default.GoogleObjectPatchData);
        request.SetRequiredGcpScope(patch.Acl?.Count > 0 ? FullControlScope : ReadWriteScope);
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);
        await HandleErrors(response).ConfigureAwait(false);
        return (await response.Content.ReadFromJsonAsync(GoogleJsonContext.Default.GoogleObjectData, cancellationToken))!;
    }

    [Obsolete("Typo: Use PatchAsync instead!")]
    public Task<GoogleObjectData> PathAsync(
        string bucket,
        string name,
        GoogleObjectPatchData patch,
        bool? includeAcl = false,
        string? accessToken = default,
        CancellationToken cancellationToken = default)
        => PatchAsync(bucket, name, patch, includeAcl, accessToken, cancellationToken);
}
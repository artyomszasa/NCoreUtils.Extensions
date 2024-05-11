using System.Net.Http.Headers;
using System.Net.Http.Json;
using NCoreUtils.Google;

namespace NCoreUtils;

public partial class GoogleCloudStorageUtils
{
    public async Task CopyAsync(
        string sourceBucket,
        string sourceName,
        string destinationBucket,
        string destinationName,
        string contentType,
        string? cacheControl = default,
        bool isPublic = true,
        string? accessToken = default,
        CancellationToken cancellationToken = default)
    {
        // prepare data
        var obj = new GoogleObjectData
        {
            ContentType = contentType,
            CacheControl = cacheControl ?? DefaultCacheControl
        };
        if (isPublic)
        {
            obj.Acl.Add(new GoogleAccessControlEntry
            {
                Entity = "allUsers",
                Role = "READER"
            });
        }
        // create request
        using var request = new HttpRequestMessage(HttpMethod.Post, EndpointFactory.Copy(sourceBucket, sourceName, destinationBucket, destinationName))
        {
            Content = new JsonContent<GoogleObjectData>(obj, GoogleJsonContext.Default.GoogleObjectData)
        };
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        request.SetRequiredGcpScope(obj.Acl.Count > 0 ? FullControlScope : ReadWriteScope);
        // perform request
        using var client = CreateHttpClient();
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }
}
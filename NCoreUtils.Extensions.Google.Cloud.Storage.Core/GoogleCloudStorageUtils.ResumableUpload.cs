using System.Net.Http.Headers;
using System.Net.Http.Json;
using NCoreUtils.Google;

namespace NCoreUtils;

public partial class GoogleCloudStorageUtils
{
    public async Task<string> InitializeResumableUploadAsync(
        string bucket,
        string name,
        string? contentType = default,
        string? cacheControl = default,
        string? origin = default,
        IEnumerable<GoogleAccessControlEntry>? acl = default,
        string? accessToken = default,
        CancellationToken cancellationToken = default)
    {
        // prepare data
        var obj = new GoogleObjectData
        {
            Name = name,
            ContentType = contentType,
            CacheControl = cacheControl ?? DefaultCacheControl,
        };
        if (acl is not null)
        {
            obj.Acl.AddRange(acl);
        }
        // create request
        using var request = new HttpRequestMessage(HttpMethod.Post, EndpointFactory.ResumableUpload(bucket))
        {
            Content = new JsonContent<GoogleObjectData>(obj, GoogleJsonContext.Default.GoogleObjectData)
        };
        // adjust headers
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        if (!string.IsNullOrEmpty(origin))
        {
            request.Headers.Add("Origin", origin);
        }
        if (!string.IsNullOrEmpty(contentType))
        {
            request.Headers.Add("X-Upload-Content-Type", contentType);
        }
        request.SetRequiredGcpScope(obj.Acl.Count > 0 ? FullControlScope : ReadWriteScope);
        using var client = CreateHttpClient();
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);
        // handle generic errors
        await HandleErrors(response).ConfigureAwait(false);
        // handle request specific errors
        if (response.Headers.Location is null)
        {
            throw new GoogleCloudStorageUploadException("No location has been returned when creating resumable upload enpoint.");
        }
        // populate result
        return response.Headers.Location.AbsoluteUri;
    }

    public Task<string> InitializeResumableUploadAsync(
        string bucket,
        string name,
        string? contentType = default,
        string? cacheControl = default,
        string? origin = default,
        bool isPublic = true,
        string? accessToken = default,
        CancellationToken cancellationToken = default)
        => InitializeResumableUploadAsync(
            bucket: bucket,
            name: name,
            contentType: contentType,
            cacheControl: cacheControl,
            origin: origin,
            acl: isPublic ? _publicRead : default,
            accessToken: accessToken,
            cancellationToken: cancellationToken
        );
}
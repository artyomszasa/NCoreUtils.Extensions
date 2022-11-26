using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using NCoreUtils.Google;

namespace NCoreUtils;

public partial class GoogleCloudStorageUtils
{
    public const string HttpClientConfigurationName = nameof(GoogleCloudStorageUtils);

    public const string ReadOnlyScope = "https://www.googleapis.com/auth/devstorage.read_only";

    public const string ReadWriteScope = "https://www.googleapis.com/auth/devstorage.read_write";

    public const string FullControlScope = "https://www.googleapis.com/auth/devstorage.full_control";

    private static readonly IReadOnlyList<GoogleAccessControlEntry> _publicRead = new GoogleAccessControlEntry[]
    {
        new GoogleAccessControlEntry
        {
            Entity = "allUsers",
            Role = "READER"
        }
    };

    private static async ValueTask HandleErrors(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = response.Content is null
                ? null
                : await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (string.IsNullOrEmpty(responseContent))
            {
                throw new InvalidOperationException($"Server responded with status code {response.StatusCode}.");
            }
            throw new InvalidOperationException($"Server responded with status code {response.StatusCode} and message: {responseContent}.");
        }
    }

    public static string DefaultCacheControl => "private, max-age=31536000";

    private IHttpClientFactory? HttpClientFactory { get; }

    public GoogleCloudStorageUtils(IHttpClientFactory? httpClientFactory)
    {
        HttpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Creates new http client. When <see cref="IHttpClientFactory" /> is provided the client is
    /// configured using the configuration that responds to the the logical name specified by
    /// <see cref="HttpClientConfigurationName" />.
    /// </summary>
    /// <returns>Configured http client.</returns>
    public HttpClient CreateHttpClient()
    {
        if (HttpClientFactory is null)
        {
            return new HttpClient();
        }
        return HttpClientFactory.CreateClient(HttpClientConfigurationName);
    }

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
        request.SetRequiredGSCScope(ReadWriteScope);
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

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
        request.SetRequiredGSCScope(ReadOnlyScope);
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
        request.SetRequiredGSCScope(ReadOnlyScope);
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

    public async Task<GoogleObjectData> PathAsync(
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
        request.SetRequiredGSCScope(patch.Acl?.Count > 0 ? FullControlScope : ReadWriteScope);
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);
        await HandleErrors(response).ConfigureAwait(false);
        return (await response.Content.ReadFromJsonAsync(GoogleJsonContext.Default.GoogleObjectData, cancellationToken))!;
    }

    public async Task<GoogleObjectData> UploadAsync(
        string bucket,
        string name,
        Stream source,
        string? contentType = default,
        string? cacheControl = default,
        IEnumerable<GoogleAccessControlEntry>? acl = default,
        string? accessToken = default,
        Action<long>? progress = default,
        IMemoryOwner<byte>? buffer = default,
        CancellationToken cancellationToken = default)
    {
        // prepare data
        var acl1 = acl is null ? default : acl.ToList();
        // initialize resumable upload
        var endpoint = await InitializeResumableUploadAsync(
            bucket: bucket,
            name: name,
            contentType: contentType,
            cacheControl: cacheControl,
            origin: default,
            acl: acl1,
            accessToken: accessToken,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        // configure uploader
        await using var uploader = new GoogleCloudStorageUploader(
            CreateHttpClient(),
            endpoint,
            contentType: contentType,
            buffer
        );
        // configure progress observere
        var size = 0L;
        uploader.Progress += (_, e) =>
        {
            progress?.Invoke(e.Sent);
            size = e.Sent;
        };
        // upload object
        await uploader.UploadAsync(source, true, cancellationToken).ConfigureAwait(false);
        // create result objects
        var obj = new GoogleObjectData
        {
            BucketName = bucket,
            Name = name,
            ContentType = contentType,
            CacheControl = cacheControl ?? DefaultCacheControl,
            Size = unchecked((ulong)size),
        };
        if (acl1 is not null)
        {
            obj.Acl.AddRange(acl1);
        }
        return obj;
    }

    public Task<GoogleObjectData> UploadAsync(
        string bucket,
        string name,
        Stream source,
        string? contentType = default,
        string? cacheControl = default,
        bool isPublic = true,
        string? accessToken = default,
        Action<long>? progress = default,
        IMemoryOwner<byte>? buffer = default,
        CancellationToken cancellationToken = default)
        => UploadAsync(
            bucket,
            name,
            source,
            contentType,
            cacheControl,
            isPublic ? _publicRead : default,
            accessToken,
            progress,
            buffer,
            cancellationToken
        );
}
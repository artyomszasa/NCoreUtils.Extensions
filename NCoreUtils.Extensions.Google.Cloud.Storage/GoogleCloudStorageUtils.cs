using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NCoreUtils.Google;

namespace NCoreUtils
{
    public class GoogleCloudStorageUtils
    {
        public const string HttpClientConfigurationName = nameof(GoogleCloudStorageUtils);

        public const string ReadOnlyScope = "https://www.googleapis.com/auth/devstorage.read_only";

        public const string ReadWriteScope = "https://www.googleapis.com/auth/devstorage.read_write";

        public const string FullControlScope = "https://www.googleapis.com/auth/devstorage.full_control";

        private static readonly MediaTypeHeaderValue _jsonMediaType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true,
            WriteIndented = false
        };

        private static readonly IReadOnlyList<GoogleAccessControlEntry> _publicRead = new GoogleAccessControlEntry[]
        {
            new GoogleAccessControlEntry
            {
                Entity = "allUsers",
                Role = "READER"
            }
        };

        public static string DefaultCacheControl => "private, max-age=31536000";

        private readonly IHttpClientFactory _httpClientFactory;

        public GoogleCloudStorageUtils(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        private async ValueTask HandleErrors(HttpResponseMessage response)
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

        /// <summary>
        /// Creates new http client. When <see cref="System.Net.Http.IHttpClientFactory" /> is provided the client is
        /// configured using the configuration that responds to the the logical name specified by
        /// <see cref="NCoreUtils.GoogleCloudStorageUtils.HttpClientConfigurationName" />.
        /// </summary>
        /// <returns>Configured http client.</returns>
        public HttpClient CreateHttpClient()
        {
            if (_httpClientFactory is null)
            {
                return new HttpClient();
            }
            return _httpClientFactory.CreateClient(HttpClientConfigurationName);
        }

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
            var requestUri = $"https://www.googleapis.com/storage/v1/b/{sourceBucket}/o/{Uri.EscapeDataString(sourceName)}/copyTo/b/{destinationBucket}/o/{Uri.EscapeDataString(destinationName)}";
            using var client = CreateHttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
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
            request.Content = JsonContent.Create(obj, _jsonMediaType, _jsonOptions);
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            request.SetRequiredGSCScope(obj.Acl.Count > 0 ? FullControlScope : ReadWriteScope);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(
            string bucket,
            string name,
            string? accessToken = default,
            CancellationToken cancellationToken = default)
        {
            var requestUri = $"https://www.googleapis.com/storage/v1/b/{bucket}/o/{Uri.EscapeDataString(name)}";
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
            var requestUri = $"https://www.googleapis.com/storage/v1/b/{bucket}/o/{Uri.EscapeDataString(name)}?alt=media";
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
                .CopyToAsync(destination)
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
            var requestUri = $"https://www.googleapis.com/storage/v1/b/{bucket}/o/{Uri.EscapeDataString(name)}";
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
                .ReadFromJsonAsync<GoogleObjectData>(_jsonOptions, cancellationToken)
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
            var projection = includeAcl.HasValue
                ? $"?projection={(includeAcl.Value ? "full" : "noAcl")}"
                : string.Empty;
            var requestUri = $"https://storage.googleapis.com/storage/v1/b/{bucket}/o/{Uri.EscapeDataString(name)}{projection}";
            using var client = CreateHttpClient();
            using var request = new HttpRequestMessage(
#if NETSTANDARD2_1
                HttpMethod.Patch,
#else
                new HttpMethod("PATCH"),
#endif
                requestUri
            );
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            request.Content = JsonContent.Create(patch, _jsonMediaType, _jsonOptions);
            request.SetRequiredGSCScope(patch.Acl?.Count > 0 ? FullControlScope : ReadWriteScope);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);
            await HandleErrors(response).ConfigureAwait(false);
            return await response.Content.ReadFromJsonAsync<GoogleObjectData>(_jsonOptions, cancellationToken);
        }

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
            var uri = $"https://www.googleapis.com/upload/storage/v1/b/{bucket}/o?uploadType=resumable";
            using var client = CreateHttpClient();
            var obj = new GoogleObjectData
            {
                Name = name,
                ContentType = contentType,
                CacheControl = cacheControl ?? DefaultCacheControl,
            };
            if (!(acl is null))
            {
                obj.Acl.AddRange(acl);
            }
            using var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = JsonContent.Create(obj, _jsonMediaType, _jsonOptions);
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
            request.SetRequiredGSCScope(obj.Acl.Count > 0 ? FullControlScope : ReadWriteScope);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);
            await HandleErrors(response).ConfigureAwait(false);
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
            var acl1 = acl is null ? default : acl.ToList();
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
            using var uploader = new GoogleCloudStorageUploader(
                CreateHttpClient(),
                endpoint,
                contentType: contentType,
                buffer
            );
            var size = 0L;
            uploader.Progress += (_, e) =>
            {
                progress?.Invoke(e.Sent);
                size = e.Sent;
            };
            await uploader.ConumeStreamAsync(source, cancellationToken).ConfigureAwait(false);
            var obj = new GoogleObjectData
            {
                BucketName = bucket,
                Name = name,
                ContentType = contentType,
                CacheControl = cacheControl ?? DefaultCacheControl,
                Size = unchecked((ulong)size),
            };
            if (!(acl1 is null))
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

        // FIXME: make no-alloc
        private string CreateListEndpoint(string bucket, string? prefix, bool? includeAcl, string? pageToken)
        {
            var uriBuilder = new StringBuilder($"https://storage.googleapis.com/storage/v1/b/{bucket}/o");
            bool first = true;
            if (!string.IsNullOrEmpty(prefix))
            {
                uriBuilder
                    .Append('?')
                    .Append("prefix=")
                    .Append(Uri.EscapeDataString(prefix));
                first = false;
            }
            if (includeAcl.HasValue)
            {
                if (first)
                {
                    uriBuilder.Append('?');
                    first = false;
                }
                else
                {
                    uriBuilder.Append('&');
                }
                uriBuilder
                    .Append("projection=")
                    .Append(includeAcl.Value ? "full" : "noAcl");
            }
            if (!string.IsNullOrEmpty(pageToken))
            {
                if (first)
                {
                    uriBuilder.Append('?');
                    first = false;
                }
                else
                {
                    uriBuilder.Append('&');
                }
                uriBuilder
                    .Append("pageToken=")
                    .Append(Uri.EscapeDataString(pageToken));
            }
            return uriBuilder.ToString();
        }

        private async IAsyncEnumerable<GoogleObjectsPage> ListAsync(string bucket, string? prefix, bool? includeAcl, string? accessToken, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var client = CreateHttpClient();
            string? pageToken = null;
            do
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, CreateListEndpoint(bucket, prefix, includeAcl, pageToken));
                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
                request.SetRequiredGSCScope(ReadOnlyScope);
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);
                await HandleErrors(response).ConfigureAwait(false);
                var page = await response.Content
                    .ReadFromJsonAsync<GoogleObjectsData>(_jsonOptions, cancellationToken)
                    .ConfigureAwait(false);
                yield return new GoogleObjectsPage(page.Prefixes, page.Items);
                pageToken = page.NextPageToken;
            }
            while (!(pageToken is null));
        }

        public IAsyncEnumerable<GoogleObjectsPage> ListAsync(string bucket, string? prefix, bool? includeAcl = false, string? accessToken = default)
        {
            return new DelayedAsyncEnumerable<GoogleObjectsPage>(cancellationToken =>
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
    }
}
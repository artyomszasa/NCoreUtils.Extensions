using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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

        private static MediaTypeHeaderValue _jsonMediaType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true
        };

        public static string DefaultCacheControl => "private, max-age=31536000";

        private IHttpClientFactory _httpClientFactory;

        public GoogleCloudStorageUtils(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
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
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
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
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
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
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            await response
                .EnsureSuccessStatusCode()
                .Content
                .CopyToAsync(destination);
            await destination.FlushAsync(cancellationToken);
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
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }
            return await response
                .EnsureSuccessStatusCode()
                .Content
                .ReadFromJsonAsync<GoogleObjectData>(_jsonOptions, cancellationToken);
        }

        public async Task<string> InitializeResumableUploadAsync(
            string bucket,
            string name,
            string? contentType = default,
            string? cacheControl = default,
            string? origin = default,
            bool isPublic = true,
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
            if (isPublic)
            {
                obj.Acl.Add(new GoogleAccessControlEntry
                {
                    Entity = "allUsers",
                    Role = "READER"
                });
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
            using var respose = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!respose.IsSuccessStatusCode)
            {
                var responseContent = respose.Content is null
                    ? null
                    : await respose.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(responseContent))
                {
                    throw new InvalidOperationException($"Server responded with status code {respose.StatusCode}.");
                }
                throw new InvalidOperationException($"Server responded with status code {respose.StatusCode} and message: {respose}.");
            }
            return respose.Headers.Location.AbsoluteUri;
        }
    }
}
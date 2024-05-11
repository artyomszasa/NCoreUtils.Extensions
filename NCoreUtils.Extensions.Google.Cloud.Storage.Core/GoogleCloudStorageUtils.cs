using NCoreUtils.Google;

namespace NCoreUtils;

public partial class GoogleCloudStorageUtils(IHttpClientFactory? httpClientFactory)
{
    public const string HttpClientConfigurationName = nameof(GoogleCloudStorageUtils);

    public const string ReadOnlyScope = "https://www.googleapis.com/auth/devstorage.read_only";

    public const string ReadWriteScope = "https://www.googleapis.com/auth/devstorage.read_write";

    public const string FullControlScope = "https://www.googleapis.com/auth/devstorage.full_control";

    private static readonly IReadOnlyList<GoogleAccessControlEntry> _publicRead =
    [
        new GoogleAccessControlEntry
        {
            Entity = "allUsers",
            Role = "READER"
        }
    ];

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

    private IHttpClientFactory? HttpClientFactory { get; } = httpClientFactory;

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
}
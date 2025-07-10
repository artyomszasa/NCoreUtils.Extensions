
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

[JsonSerializable(typeof(TokenResponse))]
internal partial class TokenResponseSerializerContext : JsonSerializerContext { }

public class MetadataServerTokenManager(IHttpClientFactory httpClientFactory)
    : IGoogleAccessTokenProvider
    , IAsyncDisposable
    , IDisposable
{
    private string? _accessToken;

    private DateTimeOffset _expiry;

    private InterlockedBoolean AccessSync;

    private SemaphoreSlim Sync { get; } = new(1);

    protected IHttpClientFactory HttpClientFactory { get; } = httpClientFactory;

    private async ValueTask<string> GetOrFetchAccessTokenAsync(CancellationToken cancellationToken)
    {
        if (_accessToken is { Length: >0 } accessToken0 && _expiry > DateTimeOffset.Now)
        {
            return accessToken0;
        }
        await Sync.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_accessToken is { Length: >0 } accessToken && _expiry > DateTimeOffset.Now)
            {
                return accessToken;
            }
            DateTimeOffset expiry;
            (accessToken, expiry) = await DoFetchAccessTokenAsync(cancellationToken).ConfigureAwait(false);
            while (!AccessSync.TrySet()) { /* noop */ }
            try
            {
                (_accessToken, _expiry) = (accessToken, expiry);
            }
            finally
            {
                AccessSync.TryReset();
            }
            return accessToken;
        }
        finally
        {
            Sync.Release();
        }
    }

    protected virtual HttpClient CreateHttpClient()
        => HttpClientFactory switch
        {
            null => new HttpClient(),
            var factory => factory.CreateClient(nameof(MetadataServerTokenManager))
        };

    protected virtual async Task<(string AccessToken, DateTimeOffset Expiry)> DoFetchAccessTokenAsync(CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "http://metadata.google.internal/computeMetadata/v1/instance/service-accounts/default/token");
        request.Headers.Add("Metadata-Flavor", "Google");
        using var client = CreateHttpClient();
        using var response = await client
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);
        var resp = await response
            .EnsureSuccessStatusCode()
            .Content
            .ReadFromJsonAsync(TokenResponseSerializerContext.Default.TokenResponse, cancellationToken)
            .ConfigureAwait(false);
        if (resp?.AccessToken is not { Length: >0 } accessToken)
        {
            throw new InvalidOperationException("Metadata server responnded with no access token.");
        }
        var expiry = resp.ExpiresIn is TimeSpan expiresIn
            ? DateTimeOffset.Now.Add(expiresIn).Subtract(TimeSpan.FromSeconds(5)) // -5 sec to be sure...
            : DateTimeOffset.MinValue;
        return (accessToken, expiry);
    }

    public ValueTask<string> GetAccessTokenAsync(ScopeCollection scope, CancellationToken cancellationToken = default)
        => GetOrFetchAccessTokenAsync(cancellationToken);

    public bool Invalidate(ScopeCollection scope, string? accessToken)
    {
        if (accessToken is null)
        {
            while (!AccessSync.TrySet()) { /* noop */ }
            try
            {
                (_accessToken, _expiry) = (default, DateTimeOffset.MinValue);
            }
            finally
            {
                AccessSync.TryReset();
            }
            return true;
        }
        while (!AccessSync.TrySet()) { /* noop */ }
        try
        {
            if (StringComparer.InvariantCulture.Equals(accessToken, _accessToken))
            {
                (_accessToken, _expiry) = (default, DateTimeOffset.MinValue);
                return true;
            }
            return false;
        }
        finally
        {
            AccessSync.TryReset();
        }
    }

    #region disposable

    private InterlockedBoolean IsDisposed;

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && IsDisposed.TrySet())
        {
            Sync.Dispose();
        }
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        if (IsDisposed.TrySet())
        {
            Sync.Dispose();
        }
        return default;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(disposing: true);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await DisposeAsyncCore();
        Dispose(disposing: false);
    }

    #endregion
}
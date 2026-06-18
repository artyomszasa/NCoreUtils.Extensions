
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace NCoreUtils.Google;

#if NETSTANDARD2_1 || NETFRAMEWORK

internal static class HttpCompat
{
    public static Task<string> ReadAsStringAsync(this HttpContent content, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return content.ReadAsStringAsync();
    }
}

#endif

public class ServiceAccountAccessTokenManager(ILogger<ServiceAccountAccessTokenManager> logger, ServiceAccountCredentialData credential, IHttpClientFactory? httpClientFactory = default)
    : IGoogleAccessTokenProvider
{
    private sealed record AccessTokenValue(string AccessToken, DateTimeOffset Expiry);

    private InterlockedBoolean Sync;

    private Dictionary<ScopeCollection, AccessTokenValue> AccessTokens { get; } = [];

    private IHttpClientFactory? HttpClientFactory { get; } = httpClientFactory;

    private ILogger<ServiceAccountAccessTokenManager> Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));

    protected ServiceAccountCredentialData Credential { get; } = credential ?? throw new ArgumentNullException(nameof(credential));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetAccessToken(ScopeCollection scope, string accessToken, DateTimeOffset expiry)
    {
        while (!Sync.TrySet()) { /* noop */ }
        try
        {
            AccessTokens[scope] = new(accessToken, expiry);
        }
        finally
        {
            Sync.TryReset();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetAccessToken(ScopeCollection scope, string accessToken, TimeSpan expiresIn)
        // NOTE: -2 seconds to be sure
        => SetAccessToken(scope, accessToken, DateTimeOffset.Now.Add(expiresIn).Add(TimeSpan.FromSeconds(-2)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryGetAccessToken(ScopeCollection scope, [MaybeNullWhen(false)] out string accessToken)
    {
        while (!Sync.TrySet()) { /* noop */ }
        try
        {
            if (AccessTokens.TryGetValue(scope, out var tokenValue)
                && tokenValue is AccessTokenValue { AccessToken: var token, Expiry: var expiry }
                && !string.IsNullOrEmpty(token) && expiry > DateTimeOffset.Now)
            {
                accessToken = token;
                return true;
            }
            accessToken = default;
            return false;
        }
        finally
        {
            Sync.TryReset();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryResetAccessToken(ScopeCollection scope, string? accessToken)
    {
        while (!Sync.TrySet()) { /* noop */ }
        try
        {
            if (accessToken is null || (AccessTokens.TryGetValue(scope, out var tokenValue)
                && tokenValue is AccessTokenValue { AccessToken: var token }
                && token == accessToken))
            {
                AccessTokens.Remove(scope);
                return true;
            }
            return false;
        }
        finally
        {
            Sync.TryReset();
        }
    }

    protected virtual HttpClient CreateHttpClient()
        => HttpClientFactory switch
        {
            null => new HttpClient(),
            var factory => factory.CreateClient(nameof(ServiceAccountAccessTokenManager))
        };

    protected virtual string CreateAssertion(ScopeCollection scope)
        => JwtHelper.CreateJwtToken(Credential, scope);

    protected async Task<string> DoGetAccessTokenAsync(ScopeCollection scope, CancellationToken cancellationToken = default)
    {
        var assertion = CreateAssertion(scope);
        using var request = new HttpRequestMessage(HttpMethod.Post, Credential.TokenUri)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
                { "assertion", assertion }
            })
        };
        using var client = CreateHttpClient();
        using var response = await client
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);
        TokenResponse resp;
        if (Logger.IsEnabled(LogLevel.Debug))
        {
            var rawResponse = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            Logger.LogDebug("Token response: '{Json}'.", rawResponse);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    resp = JsonSerializer.Deserialize(rawResponse, TokenResponseSerializerContext.Default.TokenResponse)
                        ?? throw new InvalidOperationException("Token request resulted in null.");
                }
                catch (Exception exn)
                {
                    throw new InvalidOperationException("Failed to read token response.", exn);
                }
            }
            else
            {
#if NET6_0_OR_GREATER
                throw new HttpRequestException(
                    S.Create(CultureInfo.InvariantCulture, $"Server responded with non-successful status code {response.StatusCode}'."),
                    null,
                    response.StatusCode
                );
#else
                throw new HttpRequestException(
                    S.Create(CultureInfo.InvariantCulture, $"Server responded with non-successful status code {response.StatusCode}'.")
                );
#endif
            }
        }
        else
        {
            resp = await response.EnsureSuccessStatusCode()
                .Content
                .ReadFromJsonAsync(TokenResponseSerializerContext.Default.TokenResponse, cancellationToken)
                ?? throw new InvalidOperationException("Token request resulted in null.");
        }
        if (string.IsNullOrEmpty(resp.AccessToken))
        {
            throw new InvalidOperationException("Token response contains no access token.");
        }
        SetAccessToken(scope, resp.AccessToken, resp.ExpiresIn);
        return resp.AccessToken;
    }

    public ValueTask<string> GetAccessTokenAsync(ScopeCollection scope, CancellationToken cancellationToken = default)
    {
        if (TryGetAccessToken(scope, out var accessToken))
        {
            return new(accessToken);
        }
        return new(DoGetAccessTokenAsync(scope, cancellationToken));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Invalidate(ScopeCollection scope, string? accessToken)
        => TryResetAccessToken(scope, accessToken);
}
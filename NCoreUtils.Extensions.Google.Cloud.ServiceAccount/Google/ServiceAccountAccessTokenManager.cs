
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

[JsonSerializable(typeof(TokenResponse))]
internal partial class TokenResponseSerializerContext : JsonSerializerContext { }

public class ServiceAccountAccessTokenManager(ServiceAccountCredentialData credential, IHttpClientFactory? httpClientFactory = default)
    : IGoogleAccessTokenProvider
{
    private sealed record AccessTokenValue(string AccessToken, DateTimeOffset Expiry);

    private InterlockedBoolean Sync;

    private Dictionary<ScopeCollection, AccessTokenValue> AccessTokens { get; } = [];

    private ServiceAccountCredentialData Credential { get; } = credential ?? throw new ArgumentNullException(nameof(credential));

    private IHttpClientFactory? HttpClientFactory { get; } = httpClientFactory;

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

    protected async Task<string> DoGetAccessTokenAsync(ScopeCollection scope, CancellationToken cancellationToken = default)
    {
        var jwtToken = JwtHelper.CreateJwtToken(Credential, scope);
        using var request = new HttpRequestMessage(HttpMethod.Post, Credential.TokenUri)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
                { "assertion", jwtToken }
            })
        };
        using var client = CreateHttpClient();
        using var response = await client
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);
        var resp = await response.EnsureSuccessStatusCode()
            .Content
            .ReadFromJsonAsync(TokenResponseSerializerContext.Default.TokenResponse, cancellationToken)
            ?? throw new InvalidOperationException("Token request resulted in null.");
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
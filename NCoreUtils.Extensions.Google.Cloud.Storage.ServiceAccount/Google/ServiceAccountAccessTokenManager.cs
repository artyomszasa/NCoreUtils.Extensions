
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
    private sealed class ScopeArrayComparer : IEqualityComparer<string[]>
    {
        private static IEqualityComparer<HashSet<string>> SetComparer { get; } = HashSet<string>.CreateSetComparer();

        private static FixSizePool<HashSet<string>> HashSetPool { get; } = new(16);

        public static ScopeArrayComparer Singleton { get; } = new();

        private ScopeArrayComparer() { }

        public bool Equals(string[]? x, string[]? y)
        {
            if (x is null)
            {
                return y is null;
            }
            if (y is null || x.Length != y.Length)
            {
                return false;
            }
            if (x.Length == 1 && y.Length == 1)
            {
                return x[0] == y[0];
            }
            var set = HashSetPool.TryRent(out var instance) ? instance : new(StringComparer.InvariantCulture);
            try
            {
                foreach (var item in x)
                {
                    set.Add(item);
                }
                return set.SetEquals(y);
            }
            finally
            {
                set.Clear();
                HashSetPool.Return(set);
            }
        }

        public int GetHashCode([DisallowNull] string[] obj)
        {
            if (obj is null || obj.Length == 0)
            {
                return default;
            }
            if (obj.Length == 1)
            {
                return HashCode.Combine(0, obj[0]);
            }
            var set = HashSetPool.TryRent(out var instance) ? instance : new(StringComparer.InvariantCulture);
            try
            {
                foreach (var item in obj)
                {
                    set.Add(item);
                }
                return HashCode.Combine(1, SetComparer.GetHashCode(set));
            }
            finally
            {
                set.Clear();
                HashSetPool.Return(set);
            }
        }
    }

    private sealed record AccessTokenValue(string AccessToken, DateTimeOffset Expiry);

    private InterlockedBoolean Sync;

    private Dictionary<string[], AccessTokenValue> AccessTokens { get; } = new(ScopeArrayComparer.Singleton);

    private ServiceAccountCredentialData Credential { get; } = credential ?? throw new ArgumentNullException(nameof(credential));

    private IHttpClientFactory? HttpClientFactory { get; } = httpClientFactory;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetAccessToken(string[] scope, string accessToken, DateTimeOffset expiry)
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
    private void SetAccessToken(string[] scope, string accessToken, TimeSpan expiresIn)
        // NOTE: -2 seconds to be sure
        => SetAccessToken(scope, accessToken, DateTimeOffset.Now.Add(expiresIn).Add(TimeSpan.FromSeconds(-2)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryGetAccessToken(string[] scope, [MaybeNullWhen(false)] out string accessToken)
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
    private bool TryResetAccessToken(string[] scope, string? accessToken)
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

    protected async Task<string> DoGetAccessTokenAsync(string[] scope, CancellationToken cancellationToken = default)
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

    public ValueTask<string> GetAccessTokenAsync(string[] scope, CancellationToken cancellationToken = default)
    {
        if (TryGetAccessToken(scope, out var accessToken))
        {
            return new(accessToken);
        }
        return new(DoGetAccessTokenAsync(scope, cancellationToken));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Invalidate(string[] scope, string? accessToken)
        => TryResetAccessToken(scope, accessToken);
}
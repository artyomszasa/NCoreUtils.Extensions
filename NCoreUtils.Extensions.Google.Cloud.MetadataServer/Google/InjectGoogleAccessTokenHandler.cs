using System.Net.Http.Headers;

namespace NCoreUtils.Google;

public class InjectGoogleAccessTokenHandler(IGoogleAccessTokenProvider accessTokenProvider) : DelegatingHandler
{
    public IGoogleAccessTokenProvider AccessTokenProvider { get; } = accessTokenProvider ?? throw new ArgumentNullException(nameof(accessTokenProvider));

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!HasAuthorizationData(request))
        {
            var scopes = request.TryGetRequiredGcpScope(out var scopes0) ? scopes0 : default;
            var accessToken = await AccessTokenProvider.GetAccessTokenAsync(scopes, cancellationToken).ConfigureAwait(false);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var resp = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized
                && AccessTokenProvider is MetadataServerTokenManager mgr
                && mgr.Invalidate(scopes, accessToken))
            {
                accessToken = await AccessTokenProvider.GetAccessTokenAsync(scopes, cancellationToken).ConfigureAwait(false);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            return resp;
        }
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        static bool HasAuthorizationData(HttpRequestMessage request)
        {
            var authorization = request.Headers.Authorization;
            return authorization is not null && !string.IsNullOrEmpty(authorization.Parameter);
        }
    }
}
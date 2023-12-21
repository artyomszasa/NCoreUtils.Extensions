using System.Net.Http.Headers;

namespace NCoreUtils.Google;

public class InjectGoogleAccessTokenHandler : DelegatingHandler
{
    public IGoogleAccessTokenProvider AccessTokenProvider { get; }

    public InjectGoogleAccessTokenHandler(IGoogleAccessTokenProvider accessTokenProvider)
    {
        AccessTokenProvider = accessTokenProvider ?? throw new ArgumentNullException(nameof(accessTokenProvider));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization is null || string.IsNullOrEmpty(request.Headers.Authorization.Parameter))
        {
            var scope = request.TryGetRequiredGCSScope(out var requiredScope)
                ? requiredScope
                : request.Method == HttpMethod.Get ? GoogleCloudStorageUtils.ReadOnlyScope : GoogleCloudStorageUtils.ReadWriteScope;
            var accessToken = await AccessTokenProvider.GetAccessTokenAsync(scope, cancellationToken).ConfigureAwait(false);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var resp = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized
                && AccessTokenProvider is ServiceAccountAccessTokenManager mgr
                && mgr.Invalidate(scope, accessToken))
            {
                accessToken = await AccessTokenProvider.GetAccessTokenAsync(scope, cancellationToken).ConfigureAwait(false);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            return resp;
        }
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
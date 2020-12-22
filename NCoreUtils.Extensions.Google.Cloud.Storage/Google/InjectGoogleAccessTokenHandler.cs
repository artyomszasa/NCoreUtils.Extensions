using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreUtils.Google
{
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
                var accessToken = await AccessTokenProvider.GetAccessTokenAsync(scope, cancellationToken);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
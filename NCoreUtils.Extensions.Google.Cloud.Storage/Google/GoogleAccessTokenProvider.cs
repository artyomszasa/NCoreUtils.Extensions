using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;

namespace NCoreUtils.Google
{
    public class GoogleAccessTokenProvider : IGoogleAccessTokenProvider
    {
        private GoogleCredential? _googleCredential;

        public GoogleAccessTokenProvider(GoogleCredential? googleCredential = default)
            => _googleCredential = googleCredential;

        public async ValueTask<string> GetAccessTokenAsync(ScopeCollection scopes, CancellationToken cancellationToken)
        {
            if (null == _googleCredential)
            {
                _googleCredential = await GoogleCredential.GetApplicationDefaultAsync(cancellationToken).ConfigureAwait(false);
            }
            return await _googleCredential
                .CreateScoped(scopes.ToArray())
                .UnderlyingCredential
                .GetAccessTokenForRequestAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
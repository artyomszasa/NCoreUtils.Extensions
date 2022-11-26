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

        public ValueTask<string> GetAccessTokenAsync(string[] scopes, CancellationToken cancellationToken)
            => GetAccessTokenAsync(new HashSet<string>(scopes), cancellationToken);

        public async ValueTask<string> GetAccessTokenAsync(HashSet<string> scopes, CancellationToken cancellationToken)
        {
            if (null == _googleCredential)
            {
                _googleCredential = await GoogleCredential.GetApplicationDefaultAsync(cancellationToken).ConfigureAwait(false);
            }
            return await _googleCredential
                .CreateScoped(scopes)
                .UnderlyingCredential
                .GetAccessTokenForRequestAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
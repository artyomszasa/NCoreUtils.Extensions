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

        public async ValueTask<string> GetAccessTokenAsync(string[] scopes, CancellationToken cancellationToken)
        {
            if (null == _googleCredential)
            {
                var googleCredential = await GoogleCredential.GetApplicationDefaultAsync();
                _googleCredential = googleCredential.CreateScoped(scopes);
            }
            return await _googleCredential
                .UnderlyingCredential
                .GetAccessTokenForRequestAsync(cancellationToken: cancellationToken);
        }
    }
}
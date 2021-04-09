using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;

namespace NCoreUtils.Google
{
    public class GoogleAccessTokenProvider : IGoogleAccessTokenProvider
    {
        private GoogleCredential? _googleCredential;

        private ConcurrentDictionary<HashSet<string>, GoogleCredential> _scopedCredentials
            = new ConcurrentDictionary<HashSet<string>, GoogleCredential>(HashSet<string>.CreateSetComparer());

        private Func<HashSet<string>, GoogleCredential> _scopedFactory;

        public GoogleAccessTokenProvider(GoogleCredential? googleCredential = default)
        {
            _googleCredential = googleCredential;
            _scopedFactory = scopes => _googleCredential!.CreateScoped(scopes);
        }

        public ValueTask<string> GetAccessTokenAsync(string[] scopes, CancellationToken cancellationToken)
            => GetAccessTokenAsync(new HashSet<string>(scopes), cancellationToken);

        public async ValueTask<string> GetAccessTokenAsync(HashSet<string> scopes, CancellationToken cancellationToken)
        {
            if (null == _googleCredential)
            {
                _googleCredential = await GoogleCredential.GetApplicationDefaultAsync();
            }
            var scopedCredentials = _scopedCredentials.GetOrAdd(scopes, _scopedFactory);
            return await scopedCredentials
                .UnderlyingCredential
                .GetAccessTokenForRequestAsync(cancellationToken: cancellationToken);
        }
    }
}
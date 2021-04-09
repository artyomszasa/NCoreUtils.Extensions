using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreUtils.Google
{
    public static class GoogleAccessTokenProviderExtensions
    {
        public static ValueTask<string> GetAccessTokenAsync(
            this IGoogleAccessTokenProvider tokenProvider,
            string scope,
            CancellationToken cancellationToken = default)
            => tokenProvider switch
            {
                GoogleAccessTokenProvider defaultTokenProvider => defaultTokenProvider.GetAccessTokenAsync(new HashSet<string> { scope }, cancellationToken),
                _ => tokenProvider.GetAccessTokenAsync(new [] { scope }, cancellationToken)
            };
    }
}
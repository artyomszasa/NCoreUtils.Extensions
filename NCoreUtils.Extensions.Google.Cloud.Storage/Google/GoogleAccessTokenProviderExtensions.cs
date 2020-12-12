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
            => tokenProvider.GetAccessTokenAsync(new [] { scope }, cancellationToken);
    }
}
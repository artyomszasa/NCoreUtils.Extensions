using System.Threading;
using System.Threading.Tasks;

namespace NCoreUtils.Google
{
    public interface IGoogleAccessTokenProvider
    {
        ValueTask<string> GetAccessTokenAsync(string[] scope, CancellationToken cancellationToken = default);
    }
}
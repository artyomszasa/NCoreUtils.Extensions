using System.Runtime.CompilerServices;
using NCoreUtils.Google.Wallet;

namespace NCoreUtils.Google;

public class GoogleWalletClient(IWalletV1Api api) : IGoogleWalletClient
{
    // FIXME: make configurable
    private const int DefaultMaxResultsPerRequest = 20;

    public IWalletV1Api Api { get; } = api ?? throw new ArgumentNullException(nameof(api));

    public Task<GenericClass> InsertGenericClassAsync(GenericClass data, CancellationToken cancellationToken = default)
        => Api.InsertGenericClassAsync(data, cancellationToken);

    public Task<GenericObject> InsertGenericObjectAsync(GenericObject data, CancellationToken cancellationToken = default)
        => Api.InsertGenericObjectAsync(data, cancellationToken);

    public async IAsyncEnumerable<GenericClass> ListGenericClassesAsync(
        string? issuerId = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? token = default;
        while (true)
        {
            var next = await Api.ListGenericClassesAsync(issuerId, token, DefaultMaxResultsPerRequest, cancellationToken);
            if (next.Resources is { Count: > 0 } items)
            {
                foreach (var item in items)
                {
                    yield return item;
                }
            }
            if (string.IsNullOrEmpty(next.Pagination.NextPageToken))
            {
                break;
            }
            token = next.Pagination.NextPageToken;
        }
    }

    public Task<GenericClass?> LookupGenericClass(string id, CancellationToken cancellationToken = default)
        => Api.LookupGenericClass(id, cancellationToken);
}
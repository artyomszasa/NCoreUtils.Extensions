using NCoreUtils.Google.Wallet;

namespace NCoreUtils;

public interface IGoogleWalletClient
{
    Task<GenericClass> InsertGenericClassAsync(GenericClass data, CancellationToken cancellationToken = default);

    IAsyncEnumerable<GenericClass> ListGenericClassesAsync(
        string? issuerId = default,
        CancellationToken cancellationToken = default
    );

    Task<GenericClass?> LookupGenericClass(string id, CancellationToken cancellationToken = default);

    Task<GenericObject> InsertGenericObjectAsync(GenericObject data, CancellationToken cancellationToken = default);
}
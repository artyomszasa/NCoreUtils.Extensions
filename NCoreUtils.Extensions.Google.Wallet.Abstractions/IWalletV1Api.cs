namespace NCoreUtils.Google.Wallet;

public interface IWalletV1Api
{
    Task<GenericClass> InsertGenericClassAsync(GenericClass data, CancellationToken cancellationToken = default);

    Task<ListGenericClassResponse> ListGenericClassesAsync(
        string? issuerId,
        string? token,
        int? maxResults,
        CancellationToken cancellationToken = default
    );

    Task<GenericClass?> LookupGenericClass(string id, CancellationToken cancellationToken = default);

    Task<GenericObject> InsertGenericObjectAsync(GenericObject data, CancellationToken cancellationToken = default);
}
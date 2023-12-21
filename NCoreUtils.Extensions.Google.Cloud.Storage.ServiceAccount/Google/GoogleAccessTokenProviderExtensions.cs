namespace NCoreUtils.Google;

public static class GoogleAccessTokenProviderExtensions
{
    private static string[] ReadOnlyScope { get; } = [ GoogleCloudStorageUtils.ReadOnlyScope ];

    private static string[] ReadWriteScope { get; } = [ GoogleCloudStorageUtils.ReadWriteScope ];

    private static string[] FullControlScope { get; } = [ GoogleCloudStorageUtils.FullControlScope ];

    public static ValueTask<string> GetAccessTokenAsync(
        this IGoogleAccessTokenProvider tokenProvider,
        string scope,
        CancellationToken cancellationToken = default)
    {
        var scopeArray = scope switch
        {
            GoogleCloudStorageUtils.ReadOnlyScope => ReadOnlyScope,
            GoogleCloudStorageUtils.ReadWriteScope => ReadWriteScope,
            GoogleCloudStorageUtils.FullControlScope => FullControlScope,
            _ => [scope]
        };
        return tokenProvider.GetAccessTokenAsync(scopeArray, cancellationToken);
    }

    public static bool Invalidate(
        this ServiceAccountAccessTokenManager tokenProvider,
        string scope,
        string? accessToken)
    {
        var scopeArray = scope switch
        {
            GoogleCloudStorageUtils.ReadOnlyScope => ReadOnlyScope,
            GoogleCloudStorageUtils.ReadWriteScope => ReadWriteScope,
            GoogleCloudStorageUtils.FullControlScope => FullControlScope,
            _ => [scope]
        };
        return tokenProvider.Invalidate(scopeArray, accessToken);
    }
}
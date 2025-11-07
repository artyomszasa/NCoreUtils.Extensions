using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NCoreUtils.Google;
using NCoreUtils.Google.Wallet;

namespace NCoreUtils;

public static class ServiceCollectionGoogleWalletExtensions
{
    public const string DefaultGoogleWalletServiceEndpoint = "https://walletobjects.googleapis.com";

    public static IServiceCollection AddWalletV1ApiClient(
        this IServiceCollection services,
        ServiceAccountCredentialData credentials,
        string? endpoint = default,
        bool configureHttpClient = true)
    {
        if (configureHttpClient)
        {
            services.TryAddTransient<InjectGoogleAccessTokenHandler>();
            services.AddHttpClient(WalletV1ApiClient.HttpClientConfigurationName)
                .AddHttpMessageHandler<InjectGoogleAccessTokenHandler>();
        }
        return services
            .AddGoogleCloudServiceAccount(credentials)
            .AddWalletV1ApiClient(endpoint ?? DefaultGoogleWalletServiceEndpoint, WalletV1ApiClient.HttpClientConfigurationName);
    }

    public static IServiceCollection AddWalletV1ApiClient(
        this IServiceCollection services,
        string? endpoint = default,
        bool configureHttpClient = true)
        => services.AddWalletV1ApiClient(
            credentials: ServiceAccountCredentialData.ReadDefaultAsync(CancellationToken.None).GetAwaiter().GetResult(),
            endpoint: endpoint,
            configureHttpClient: configureHttpClient
        );

    private static IServiceCollection AddWalletClientWithoutDependencies(this IServiceCollection services)
        => services.AddSingleton<IGoogleWalletClient, GoogleWalletClient>();

    public static IServiceCollection AddGoogleWalletClient(
        this IServiceCollection services,
        ServiceAccountCredentialData credentials,
        bool configureHttpClient = true)
        => services
            .AddWalletV1ApiClient(credentials, default, configureHttpClient)
            .AddWalletClientWithoutDependencies();

    public static IServiceCollection AddGoogleWalletClient(
        this IServiceCollection services,
        bool configureHttpClient = true)
        => services
            .AddWalletV1ApiClient(default, configureHttpClient)
            .AddWalletClientWithoutDependencies();
}
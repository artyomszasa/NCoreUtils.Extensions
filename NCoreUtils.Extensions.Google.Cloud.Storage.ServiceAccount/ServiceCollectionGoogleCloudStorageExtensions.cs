using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NCoreUtils.Google;

namespace NCoreUtils;

public static class ServiceCollectionGoogleCloudStorageExtensions
{
    public static IServiceCollection AddGoogleCloudStorageUtils(this IServiceCollection services, ServiceAccountCredentialData credentials, bool configureHttpClient = true)
    {
        if (configureHttpClient)
        {
            services.TryAddTransient<InjectGoogleAccessTokenHandler>();
            services.AddHttpClient(GoogleCloudStorageUtils.HttpClientConfigurationName)
                .AddHttpMessageHandler<InjectGoogleAccessTokenHandler>();
        }
        return services
            .AddGoogleCloudServiceAccount(credentials)
            .AddSingleton<GoogleCloudStorageUtils>();
    }

    public static IServiceCollection AddGoogleCloudStorageUtils(this IServiceCollection services, bool configureHttpClient = true)
        => services.AddGoogleCloudStorageUtils(
            credentials: ServiceAccountCredentialData.ReadDefaultAsync(CancellationToken.None).GetAwaiter().GetResult(),
            configureHttpClient: configureHttpClient
        );
}
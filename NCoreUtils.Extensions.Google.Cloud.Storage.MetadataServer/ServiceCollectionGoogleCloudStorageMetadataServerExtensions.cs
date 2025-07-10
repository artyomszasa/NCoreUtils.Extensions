using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NCoreUtils.Google;

namespace NCoreUtils;

public static class ServiceCollectionGoogleCloudStorageMetadataServerExtensions
{
    public static IServiceCollection AddGoogleCloudStorageUtils(
        this IServiceCollection services,
        bool configureHttpClient = true)
    {
        if (configureHttpClient)
        {
            services.TryAddTransient<InjectGoogleAccessTokenHandler>();
            services.AddHttpClient(GoogleCloudStorageUtils.HttpClientConfigurationName)
                .AddHttpMessageHandler<InjectGoogleAccessTokenHandler>();
        }
        return services
            .AddGoogleCloudMetadataServer()
            .AddSingleton<GoogleCloudStorageUtils>();
    }
}
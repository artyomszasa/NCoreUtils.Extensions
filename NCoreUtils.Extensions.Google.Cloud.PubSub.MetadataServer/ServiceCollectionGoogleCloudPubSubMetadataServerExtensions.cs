using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NCoreUtils.Google;
using NCoreUtils.Google.Cloud.PubSub;

namespace NCoreUtils;

public static class ServiceCollectionGoogleCloudPubSubMetadataServerExtensions
{
    public const string DefaultGoogleCloudPubSubServiceEndpoint = "https://pubsub.googleapis.com";

    public static IServiceCollection AddGoogleCloudPubSubClient(
        this IServiceCollection services,
        string? endpoint = default,
        bool configureHttpClient = true)
    {
        if (configureHttpClient)
        {
            services.TryAddTransient<InjectGoogleAccessTokenHandler>();
            services.AddHttpClient(PubSubV1ApiClient.HttpClientConfigurationName)
                .AddHttpMessageHandler<InjectGoogleAccessTokenHandler>();
        }
        return services
            .AddGoogleCloudMetadataServer()
            .AddPubSubV1ApiClient(endpoint ?? DefaultGoogleCloudPubSubServiceEndpoint, PubSubV1ApiClient.HttpClientConfigurationName);
    }
}
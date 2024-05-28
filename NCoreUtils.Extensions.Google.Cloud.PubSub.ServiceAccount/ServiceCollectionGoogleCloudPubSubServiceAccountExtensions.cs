using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NCoreUtils.Google;
using NCoreUtils.Google.Cloud.PubSub;

namespace NCoreUtils;

public static class ServiceCollectionGoogleCloudPubSubServiceAccountExtensions
{
    public const string DefaultGoogleCountPubSubServiceEndpoint = "https://pubsub.googleapis.com";

    public static IServiceCollection AddGoogleCloudPubSubClient(
        this IServiceCollection services,
        ServiceAccountCredentialData credentials,
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
            .AddGoogleCloudServiceAccount(credentials)
            .AddPubSubV1ApiClient(endpoint ?? DefaultGoogleCountPubSubServiceEndpoint, PubSubV1ApiClient.HttpClientConfigurationName);
    }

    public static IServiceCollection AddGoogleCloudPubSubClient(
        this IServiceCollection services,
        string? endpoint,
        bool configureHttpClient = true)
        => services.AddGoogleCloudPubSubClient(
            credentials: ServiceAccountCredentialData.ReadDefaultAsync(CancellationToken.None).GetAwaiter().GetResult(),
            endpoint: endpoint,
            configureHttpClient: configureHttpClient
        );
}
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NCoreUtils.Google;
using NCoreUtils.Google.Cloud.Monitoring;

namespace NCoreUtils;

public static class ServiceCollectionGoogleCloudMonitoringServiceAccountExtensions
{
    public const string DefaultGoogleCloudMonitoringServiceEndpoint = "https://monitoring.googleapis.com";

    public static IServiceCollection AddGoogleCloudMonitoringClient(
        this IServiceCollection services,
        ServiceAccountCredentialData credentials,
        string? endpoint = default,
        bool configureHttpClient = true)
    {
        if (configureHttpClient)
        {
            services.TryAddTransient<InjectGoogleAccessTokenHandler>();
            services.AddHttpClient(MonitoringV3ApiClient.HttpClientConfigurationName)
                .AddHttpMessageHandler<InjectGoogleAccessTokenHandler>();
        }
        return services
            .AddGoogleCloudServiceAccount(credentials)
            .AddMonitoringV3ApiClient(endpoint ?? DefaultGoogleCloudMonitoringServiceEndpoint, MonitoringV3ApiClient.HttpClientConfigurationName);
    }

    public static IServiceCollection AddGoogleCloudMonitoringClient(
        this IServiceCollection services,
        string? endpoint = default,
        bool configureHttpClient = true)
        => services.AddGoogleCloudMonitoringClient(
            credentials: ServiceAccountCredentialData.ReadDefaultAsync(CancellationToken.None).GetAwaiter().GetResult(),
            endpoint: endpoint,
            configureHttpClient: configureHttpClient
        );
}
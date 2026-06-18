using Microsoft.Extensions.DependencyInjection;
using NCoreUtils.Google;
using NCoreUtils.Google.Cloud.AgentPlatform;

namespace NCoreUtils;

public static class ServiceCollectionGoogleAgentPlatformExtensions
{
    public const string DefaultAgentPlatformServiceEndpoint = "https://aiplatform.googleapis.com";

    public static IServiceCollection AddAgentPlatformApiV1Client(
        this IServiceCollection services,
        ServiceAccountCredentialData credentials,
        string? endpoint = default,
        bool configureHttpClient = true)
    {
        if (configureHttpClient)
        {
            services.AddTransient<InjectGoogleAccessTokenHandler>();
            services.AddHttpClient(AgentPlatformApiV1Client.HttpClientConfigurationName)
                .AddHttpMessageHandler<InjectGoogleAccessTokenHandler>();
        }
        return services
            .AddGoogleCloudServiceAccount(credentials)
            .AddAgentPlatformApiV1Client(endpoint ?? DefaultAgentPlatformServiceEndpoint, AgentPlatformApiV1Client.HttpClientConfigurationName);
    }
}
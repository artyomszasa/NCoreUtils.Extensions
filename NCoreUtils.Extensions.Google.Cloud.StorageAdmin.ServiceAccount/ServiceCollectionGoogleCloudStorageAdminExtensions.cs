using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NCoreUtils.Google;

namespace NCoreUtils;

public static class ServiceCollectionGoogleCloudStorageAdminExtensions
{
    public const string DefaultGoogleCloudStorageServiceEndpoint = "https://storage.googleapis.com";

    public static IServiceCollection AddGoogleStorageAdminApiV1Client(
        this IServiceCollection services,
        ServiceAccountCredentialData credentials,
        string? endpoint = default,
        bool configureHttpClient = true)
    {
        if (configureHttpClient)
        {
            services.TryAddTransient<InjectGoogleAccessTokenHandler>();
            services.AddHttpClient(GoogleStorageAdminApiV1Client.HttpClientConfigurationName)
                .AddHttpMessageHandler<InjectGoogleAccessTokenHandler>();
        }
        return services
            .AddGoogleCloudServiceAccount(credentials)
            .AddGoogleStorageAdminApiV1Client(endpoint ?? DefaultGoogleCloudStorageServiceEndpoint, GoogleStorageAdminApiV1Client.HttpClientConfigurationName);
    }

    public static IServiceCollection AddGoogleStorageAdminApiV1Client(
        this IServiceCollection services,
        string? endpoint = default,
        bool configureHttpClient = true)
        => services.AddGoogleStorageAdminApiV1Client(
            credentials: ServiceAccountCredentialData.ReadDefaultAsync(CancellationToken.None).GetAwaiter().GetResult(),
            endpoint: endpoint,
            configureHttpClient: configureHttpClient
        );

    private static IServiceCollection AddGoogleStorageAdminClientWithoutDependencies(
        this IServiceCollection services,
        string projectId)
        => services.AddSingleton<IGoogleStorageAdminClient>(serviceProvider => new GoogleStorageAdminClient(
            api: serviceProvider.GetRequiredService<IGoogleStorageAdminApiV1>(),
            projectId: projectId
        ));

    public static IServiceCollection AddGoogleStorageAdminClient(
        this IServiceCollection services,
        string projectId,
        ServiceAccountCredentialData credentials,
        bool configureHttpClient = true)
        => services
            .AddGoogleStorageAdminApiV1Client(credentials, default, configureHttpClient)
            .AddGoogleStorageAdminClientWithoutDependencies(projectId);

    public static IServiceCollection AddGoogleStorageAdminClient(
        this IServiceCollection services,
        string projectId,
        bool configureHttpClient = true)
        => services
            .AddGoogleStorageAdminApiV1Client(default, configureHttpClient)
            .AddGoogleStorageAdminClientWithoutDependencies(projectId);
}

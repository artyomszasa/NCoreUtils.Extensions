using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NCoreUtils.Google;

namespace NCoreUtils;

public static class ServiceCollectionGoogleCertificateManagerExtensions
{
    public const string DefaultGoogleCertificacteManagerServiceEndpoint = "https://certificatemanager.googleapis.com";

    public static IServiceCollection AddGoogleCertificateManagerApiV1Client(
        this IServiceCollection services,
        ServiceAccountCredentialData credentials,
        string? endpoint = default,
        bool configureHttpClient = true)
    {
        if (configureHttpClient)
        {
            services.TryAddTransient<InjectGoogleAccessTokenHandler>();
            services.AddHttpClient(GoogleCertificateManagerApiV1Client.HttpClientConfigurationName)
                .AddHttpMessageHandler<InjectGoogleAccessTokenHandler>();
        }
        return services
            .AddGoogleCloudServiceAccount(credentials)
            .AddGoogleCertificateManagerApiV1Client(endpoint ?? DefaultGoogleCertificacteManagerServiceEndpoint, GoogleCertificateManagerApiV1Client.HttpClientConfigurationName);
    }

    public static IServiceCollection AddGoogleCertificateManagerApiV1Client(
        this IServiceCollection services,
        string? endpoint = default,
        bool configureHttpClient = true)
        => services.AddGoogleCertificateManagerApiV1Client(
            credentials: ServiceAccountCredentialData.ReadDefaultAsync(CancellationToken.None).GetAwaiter().GetResult(),
            endpoint: endpoint,
            configureHttpClient: configureHttpClient
        );

    private static IServiceCollection AddGoogleCertificateManagerClientWithoutDependencies(
        this IServiceCollection services,
        string projectId,
        string location)
        => services.AddSingleton<IGoogleCertificateManagerClient>(serviceProvider => new GoogleCertificateManagerClient(
            api: serviceProvider.GetRequiredService<IGoogleCertificateManagerApiV1>(),
            projectId: projectId,
            location: location
        ));

    public static IServiceCollection AddGoogleCertificateManagerClient(
        this IServiceCollection services,
        string projectId,
        string location,
        ServiceAccountCredentialData credentials,
        bool configureHttpClient = true)
        => services
            .AddGoogleCertificateManagerApiV1Client(credentials, default, configureHttpClient)
            .AddGoogleCertificateManagerClientWithoutDependencies(projectId, location);

    public static IServiceCollection AddGoogleCertificateManagerClient(
        this IServiceCollection services,
        string projectId,
        string location,
        bool configureHttpClient = true)
        => services
            .AddGoogleCertificateManagerApiV1Client(default, configureHttpClient)
            .AddGoogleCertificateManagerClientWithoutDependencies(projectId, location);
}

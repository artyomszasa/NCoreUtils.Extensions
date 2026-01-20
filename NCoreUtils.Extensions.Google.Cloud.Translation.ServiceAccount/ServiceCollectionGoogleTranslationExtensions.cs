using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NCoreUtils.Google;

namespace NCoreUtils;

public static class ServiceCollectionGoogleTranslationExtensions
{
    public const string DefaultGoogleTranslationServiceEndpoint = "https://translate.googleapis.com";

    public static IServiceCollection AddTranslationApiV3Client(
        this IServiceCollection services,
        ServiceAccountCredentialData credentials,
        string? endpoint = default,
        bool configureHttpClient = true)
    {
        if (configureHttpClient)
        {
            services.TryAddTransient<InjectGoogleAccessTokenHandler>();
            services.AddHttpClient(TranslationApiV3Client.HttpClientConfigurationName)
                .AddHttpMessageHandler<InjectGoogleAccessTokenHandler>();
        }
        return services
            .AddGoogleCloudServiceAccount(credentials)
            .AddTranslationApiV3Client(endpoint ?? DefaultGoogleTranslationServiceEndpoint, TranslationApiV3Client.HttpClientConfigurationName);
    }

    public static IServiceCollection AddTranslationApiV3Client(
        this IServiceCollection services,
        string? endpoint = default,
        bool configureHttpClient = true)
        => services.AddTranslationApiV3Client(
            credentials: ServiceAccountCredentialData.ReadDefaultAsync(CancellationToken.None).GetAwaiter().GetResult(),
            endpoint: endpoint,
            configureHttpClient: configureHttpClient
        );

    private static IServiceCollection AddGoogleTranslationClientWithoutDependencies(
        this IServiceCollection services,
        string projectId,
        string? location)
        => services.AddSingleton<IGoogleTranslationClient>(serviceProvider => new GoogleTranslationClient(
            api: serviceProvider.GetRequiredService<ITranslationApiV3>(),
            projectId: projectId,
            location: location
        ));

    public static IServiceCollection AddGoogleTranslationClient(
        this IServiceCollection services,
        string projectId,
        string? location,
        ServiceAccountCredentialData credentials,
        bool configureHttpClient = true)
        => services
            .AddTranslationApiV3Client(credentials, default, configureHttpClient)
            .AddGoogleTranslationClientWithoutDependencies(projectId, location);

    public static IServiceCollection AddGoogleTranslationClient(
        this IServiceCollection services,
        string projectId,
        string? location = default,
        bool configureHttpClient = true)
        => services
            .AddTranslationApiV3Client(default, configureHttpClient)
            .AddGoogleTranslationClientWithoutDependencies(projectId, location);
}

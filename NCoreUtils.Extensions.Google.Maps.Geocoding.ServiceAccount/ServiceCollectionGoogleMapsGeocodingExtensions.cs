using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NCoreUtils.Google;
using NCoreUtils.Google.Maps.Geocoding;

namespace NCoreUtils;

public static class ServiceCollectionGoogleMapsGeocodingExtensions
{
    public const string DefaultGoogleMapsGeocoodingServiceEndpoint = "https://geocode.googleapis.com";

    public static IServiceCollection AddGoogleGeocodingApiV1Client(
        this IServiceCollection services,
        ServiceAccountCredentialData credentials,
        string? endpoint = default,
        bool configureHttpClient = true)
    {
        if (configureHttpClient)
        {
            services.TryAddTransient<InjectGoogleAccessTokenHandler>();
            services.AddHttpClient(GeocodingV4BetaApiClient.HttpClientConfigurationName)
                .AddHttpMessageHandler<InjectGoogleAccessTokenHandler>();
        }
        return services
            .AddGoogleCloudServiceAccount(credentials)
            .AddGeocodingV4BetaApiClient(endpoint ?? DefaultGoogleMapsGeocoodingServiceEndpoint, GeocodingV4BetaApiClient.HttpClientConfigurationName);
    }

    public static IServiceCollection AddGoogleGeocodingApiV1Client(
        this IServiceCollection services,
        string? endpoint = default,
        bool configureHttpClient = true)
        => services.AddGoogleGeocodingApiV1Client(
            credentials: ServiceAccountCredentialData.ReadDefaultAsync(CancellationToken.None).GetAwaiter().GetResult(),
            endpoint: endpoint,
            configureHttpClient: configureHttpClient
        );

    private static IServiceCollection AddGeocodingClientWithoutDependencies(this IServiceCollection services)
        => services.AddSingleton<IGeocodingClient, GeocodingClient>();

    public static IServiceCollection AddGeocodingClient(
        this IServiceCollection services,
        ServiceAccountCredentialData credentials,
        bool configureHttpClient = true)
        => services
            .AddGoogleGeocodingApiV1Client(credentials, default, configureHttpClient)
            .AddGeocodingClientWithoutDependencies();

    public static IServiceCollection AddGeocodingClient(
        this IServiceCollection services,
        bool configureHttpClient = true)
        => services
            .AddGoogleGeocodingApiV1Client(default, configureHttpClient)
            .AddGeocodingClientWithoutDependencies();
}
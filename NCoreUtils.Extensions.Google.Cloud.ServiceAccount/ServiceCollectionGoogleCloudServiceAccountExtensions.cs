using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NCoreUtils.Google;

namespace NCoreUtils;

public static class ServiceCollectionGoogleCloudServiceAccountExtensions
{
    public static IServiceCollection AddGoogleCloudServiceAccount(
        this IServiceCollection services,
        ServiceAccountCredentialData credentials)
    {
        services.TryAddSingleton<IGoogleAccessTokenProvider, ServiceAccountAccessTokenManager>();
        return services
            .AddSingleton(credentials);
    }

    public static IServiceCollection AddGoogleCloudServiceAccount(this IServiceCollection services)
        => services.AddGoogleCloudServiceAccount(
            credentials: ServiceAccountCredentialData.ReadDefaultAsync(CancellationToken.None).GetAwaiter().GetResult()
        );
}
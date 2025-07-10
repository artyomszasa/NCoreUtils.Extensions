using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NCoreUtils.Google;

namespace NCoreUtils;

public static class ServiceCollectionGoogleCloudMetadataServerExtensions
{
    public static IServiceCollection AddGoogleCloudMetadataServer(this IServiceCollection services)
    {
        services.TryAddSingleton<IGoogleAccessTokenProvider, MetadataServerTokenManager>();
        return services;
    }
}
using System.Diagnostics.CodeAnalysis;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NCoreUtils.Google;

namespace NCoreUtils;

public static class ServiceCollectionGoogleCloudStorageExtensions
{
#if NET6_0_OR_GREATER
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonCredentialParameters))]
#endif
    public static IServiceCollection AddGoogleCloudStorageUtils(this IServiceCollection services, bool configureHttpClient = true)
    {
        if (configureHttpClient)
        {
            services.TryAddSingleton<IGoogleAccessTokenProvider, GoogleAccessTokenProvider>();
            services.AddTransient<InjectGoogleAccessTokenHandler>();
            services.AddHttpClient(GoogleCloudStorageUtils.HttpClientConfigurationName)
                .AddHttpMessageHandler<InjectGoogleAccessTokenHandler>();
        }
        return services.AddSingleton<GoogleCloudStorageUtils>();
    }
}
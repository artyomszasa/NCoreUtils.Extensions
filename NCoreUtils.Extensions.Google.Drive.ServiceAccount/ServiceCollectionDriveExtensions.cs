using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NCoreUtils.Google;
using NCoreUtils.Google.Drive;

namespace NCoreUtils;

public static class ServiceCollectionDriveExtensions
{
    private sealed class EmailWrapper(string email)
        : IEquatable<EmailWrapper>
    {
        public string Email { get; } = email;

        public bool Equals([NotNullWhen(true)] EmailWrapper? other)
            => ReferenceEquals(this, other) || (other is not null && Email == other.Email);

        public override bool Equals([NotNullWhen(true)] object? obj)
            => Equals(obj as EmailWrapper);

        public override int GetHashCode()
            => Email.GetHashCode();
    }

    private sealed class NoopDriveConfiguration : IDriveConfiguration
    {
        public static NoopDriveConfiguration Singleton => field ??= new();

        private NoopDriveConfiguration() { }

        public string AdjustScope(DriveApiV3Client.Methods method, string defaultScope) => defaultScope;
    }

    public const string DefaultDriveServiceEndpoint = "https://www.googleapis.com";

    public static IServiceCollection AddDelegatedDriveApiV3Client(
        this IServiceCollection services,
        ServiceAccountCredentialData credentials,
        string email,
        string? endpoint = default,
        bool configureHttpClient = true,
        IDriveConfiguration? configuration = default)
    {
        var key = new EmailWrapper(email);
        if (configureHttpClient)
        {
            services.AddHttpClient(DriveApiV3Client.HttpClientConfigurationName)
                .AddHttpMessageHandler(serviceProvider => new InjectGoogleAccessTokenHandler(
                    serviceProvider.GetRequiredKeyedService<IGoogleAccessTokenProvider>(key)
                ));
        }
        services.TryAddKeyedSingleton<IGoogleAccessTokenProvider>(key, (serviceProvider, _) =>
        {
            return new ServiceAccountDelegatedAccessTokenManager(
                serviceProvider.GetRequiredService<ILogger<ServiceAccountDelegatedAccessTokenManager>>(),
                credentials,
                email,
                serviceProvider.GetService<IHttpClientFactory>()
            );
        });
        return services
            .AddSingleton(configuration ?? NoopDriveConfiguration.Singleton)
            .AddDriveApiV3Client(endpoint ?? DefaultDriveServiceEndpoint, DriveApiV3Client.HttpClientConfigurationName);
    }

    public static IServiceCollection AddDelegatedDriveApiV3Client(
        this IServiceCollection services,
        string email,
        string? endpoint = default,
        bool configureHttpClient = true,
        IDriveConfiguration? configuration = default)
        => services.AddDelegatedDriveApiV3Client(
            email: email,
            credentials: ServiceAccountCredentialData.ReadDefaultAsync(CancellationToken.None).GetAwaiter().GetResult(),
            endpoint: endpoint,
            configureHttpClient: configureHttpClient,
            configuration: configuration
        );

    private static IServiceCollection AddDelegatedDriveClientWithoutDependencies(this IServiceCollection services)
        => services.AddSingleton<IDriveClient>(serviceProvider => new DriveClient(
            api: serviceProvider.GetRequiredService<IDriveApiV3>(),
            httpClientFactory: serviceProvider.GetService<IHttpClientFactory>()
        ));

    public static IServiceCollection AddDelegatedDriveClient(
        this IServiceCollection services,
        ServiceAccountCredentialData credentials,
        string email,
        bool configureHttpClient = true,
        IDriveConfiguration? configuration = default)
        => services
            .AddDelegatedDriveApiV3Client(credentials, email, default, configureHttpClient, configuration)
            .AddDelegatedDriveClientWithoutDependencies();
}
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NCoreUtils.Google;
using NCoreUtils.Google.Gmail;

namespace NCoreUtils;

public static class ServiceCollectionGmailExtensions
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

    private sealed class NoopGmailConfiguration : IGmailConfiguration
    {
        public static NoopGmailConfiguration Singleton => field ??= new();

        private NoopGmailConfiguration() { }

        public string AdjustScope(GmailApiV1Client.Methods method, string defaultScope) => defaultScope;
    }

    public const string DefaultGmailServiceEndpoint = "https://gmail.googleapis.com";

    public static IServiceCollection AddDelegatedGmailApiV1Client(
        this IServiceCollection services,
        ServiceAccountCredentialData credentials,
        string email,
        string? endpoint = default,
        bool configureHttpClient = true,
        IGmailConfiguration? configuration = default)
    {
        var key = new EmailWrapper(email);
        if (configureHttpClient)
        {
            services.AddHttpClient(GmailApiV1Client.HttpClientConfigurationName)
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
            .AddSingleton(configuration ?? NoopGmailConfiguration.Singleton)
            .AddGmailApiV1Client(endpoint ?? DefaultGmailServiceEndpoint, GmailApiV1Client.HttpClientConfigurationName);
    }

    public static IServiceCollection AddDelegatedGmailApiV1Client(
        this IServiceCollection services,
        string email,
        string? endpoint = default,
        bool configureHttpClient = true,
        IGmailConfiguration? configuration = default)
        => services.AddDelegatedGmailApiV1Client(
            email: email,
            credentials: ServiceAccountCredentialData.ReadDefaultAsync(CancellationToken.None).GetAwaiter().GetResult(),
            endpoint: endpoint,
            configureHttpClient: configureHttpClient,
            configuration: configuration
        );

    private static IServiceCollection AddDelegatedGmailClientWithoutDependencies(this IServiceCollection services)
        => services.AddSingleton<IGmailClient>(serviceProvider => new GmailClient(
            api: serviceProvider.GetRequiredService<IGmailApiV1>()
        ));

    public static IServiceCollection AddDelegatedGmailClient(
        this IServiceCollection services,
        ServiceAccountCredentialData credentials,
        string email,
        bool configureHttpClient = true,
        IGmailConfiguration? configuration = default)
        => services
            .AddDelegatedGmailApiV1Client(credentials, email, default, configureHttpClient, configuration)
            .AddDelegatedGmailClientWithoutDependencies();
}
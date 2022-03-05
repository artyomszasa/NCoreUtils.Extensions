using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace NCoreUtils;

public static class ServiceCollectionTrimExtensions
{
    public static IServiceCollection AddSingletonUntrimmed<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TImplementation>(
        this IServiceCollection services)
        where TImplementation : class
        => services.AddSingleton<TImplementation>();

    public static IServiceCollection AddSingletonUntrimmed<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TImplementation>(
        this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
        => services.AddSingleton<TService, TImplementation>();

    public static IServiceCollection AddSingletonUntrimmed(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type implementationType)
        => services.AddSingleton(implementationType);

    public static IServiceCollection AddSingletonUntrimmed(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type implementationType)
        => services.AddSingleton(serviceType, implementationType);

    public static IServiceCollection AddScopedUntrimmed<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TImplementation>(
        this IServiceCollection services)
        where TImplementation : class
        => services.AddScoped<TImplementation>();

    public static IServiceCollection AddScopedUntrimmed<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TImplementation>(
        this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
        => services.AddScoped<TService, TImplementation>();

    public static IServiceCollection AddScopedUntrimmed(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type implementationType)
        => services.AddScoped(implementationType);

    public static IServiceCollection AddScopedUntrimmed(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type implementationType)
        => services.AddScoped(serviceType, implementationType);

    public static IServiceCollection AddTransientUntrimmed<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TImplementation>(
        this IServiceCollection services)
        where TImplementation : class
        => services.AddTransient<TImplementation>();

    public static IServiceCollection AddTransientUntrimmed<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TImplementation>(
        this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
        => services.AddTransient<TService, TImplementation>();

    public static IServiceCollection AddTransientUntrimmed(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type implementationType)
        => services.AddTransient(implementationType);

    public static IServiceCollection AddTransientUntrimmed(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type implementationType)
        => services.AddTransient(serviceType, implementationType);
}
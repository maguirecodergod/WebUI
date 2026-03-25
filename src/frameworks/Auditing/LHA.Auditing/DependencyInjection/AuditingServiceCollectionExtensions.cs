using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using LHA.Auditing.Interceptors;
using Castle.DynamicProxy;
using LHA.Ddd.Application;

namespace LHA.Auditing;

/// <summary>
/// Registers LHA auditing services in the DI container.
/// </summary>
internal static class AuditingServiceCollectionExtensions
{
    /// <summary>
    /// Adds core LHA auditing services:
    /// <list type="bullet">
    ///   <item><see cref="IAuditingManager"/> — scope management with <see cref="AsyncLocal{T}"/></item>
    ///   <item><see cref="IAuditPropertySetter"/> — sets audit properties on entities</item>
    ///   <item><see cref="IAuditSerializer"/> — JSON serialization for parameters</item>
    ///   <item><see cref="IAuditingStore"/> — logging fallback (replace with your persistence)</item>
    ///   <item><see cref="IAuditUserProvider"/> — null provider (replace with your identity bridge)</item>
    /// </list>
    /// </summary>
    public static IServiceCollection AddLHAAuditing(
        this IServiceCollection services,
        Action<AuditingOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.TryAddSingleton<IAuditUserProvider, NullAuditUserProvider>();
        services.TryAddSingleton<IAuditingStore, LoggingAuditingStore>();
        services.TryAddSingleton<IAuditSerializer, JsonAuditSerializer>();
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddScoped<IAuditingManager, AuditingManager>();
        services.TryAddTransient<IAuditPropertySetter, AuditPropertySetter>();

        // Interception infrastructure
        services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();
        services.TryAddTransient<AuditingInterceptor>();

        return services;
    }

    /// <summary>
    /// Swaps existing registrations of <see cref="IApplicationService"/> with intercepted proxies.
    /// Should be called after all services have been registered, usually at the end of Program.cs.
    /// </summary>
    internal static IServiceCollection AddAuditingInterception(this IServiceCollection services)
    {
        // Find all interface registrations for types implementing IApplicationService
        var descriptors = services
            .Where(d => d.ServiceType.IsInterface && 
                        typeof(IApplicationService).IsAssignableFrom(d.ServiceType) &&
                        d.ServiceType != typeof(IApplicationService) &&
                        !d.ServiceType.IsGenericType)
            .ToList();

        foreach (var descriptor in descriptors)
        {
            var serviceType = descriptor.ServiceType;
            var implementationType = descriptor.ImplementationType;
            var implementationInstance = descriptor.ImplementationInstance;
            var implementationFactory = descriptor.ImplementationFactory;

            // We replace with a factory that resolves the target and wraps it in a proxy
            var factoryDescriptor = ServiceDescriptor.Describe(serviceType, sp =>
            {
                var generator = sp.GetRequiredService<IProxyGenerator>();
                var interceptor = sp.GetRequiredService<AuditingInterceptor>();

                // Resolve the "real" instance
                object target;
                if (implementationType != null)
                    target = sp.GetServiceOrCreateInstance(implementationType);
                else if (implementationInstance != null)
                    target = implementationInstance;
                else if (implementationFactory != null)
                    target = implementationFactory(sp);
                else
                    throw new InvalidOperationException($"Could not resolve implementation for {serviceType}");

                return generator.CreateInterfaceProxyWithTargetInterface(serviceType, target, interceptor);
            }, descriptor.Lifetime);

            // Need to remove previous and add new (can't easily replace by index during enumeration)
            services.Remove(descriptor);
            services.Add(factoryDescriptor);
        }

        return services;
    }

    private static object GetServiceOrCreateInstance(this IServiceProvider sp, Type implementationType)
    {
        // Try to resolve it from the container first (in case it was also registered as concrete)
        var instance = sp.GetService(implementationType);
        if (instance != null) return instance;

        // Otherwise create it manually using DI
        return ActivatorUtilities.CreateInstance(sp, implementationType);
    }
}

internal static class DataAuditingApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="LHA.Auditing.Interceptors.DataAuditingMiddleware"/> 
    /// to the application's request pipeline, enabling integration with the classic 
    /// EF Core Entity tracking system.
    /// </summary>
    internal static IApplicationBuilder UseLHADataAuditing(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<LHA.Auditing.Interceptors.DataAuditingMiddleware>();
    }
}

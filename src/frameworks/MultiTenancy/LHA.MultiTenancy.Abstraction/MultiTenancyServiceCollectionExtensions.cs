using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.MultiTenancy;

/// <summary>
/// Extension methods for registering multi-tenancy abstractions.
/// </summary>
public static class MultiTenancyServiceCollectionExtensions
{
    /// <summary>
    /// Registers the multi-tenancy infrastructure with in-memory defaults.
    /// </summary>
    public static IServiceCollection AddLHAMultiTenancy(
        this IServiceCollection services,
        Action<MultiTenancyOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.Configure<TenantCircuitBreakerOptions>(_ => { });

        services.TryAddSingleton<ICurrentTenantAccessor, AsyncLocalCurrentTenantAccessor>();
        services.TryAddTransient<ICurrentTenant, CurrentTenant>();
        services.TryAddTransient<ITenantResolver, TenantResolver>();
        services.TryAddSingleton<ITenantCircuitBreaker, TenantCircuitBreaker>();
        services.TryAddSingleton<ITenantStore, NullTenantStore>();

        return services;
    }
}

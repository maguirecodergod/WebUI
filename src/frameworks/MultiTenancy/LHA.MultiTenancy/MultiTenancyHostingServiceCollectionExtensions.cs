using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.MultiTenancy;

/// <summary>
/// Extensions that register the full multi-tenancy hosting stack,
/// including configuration-based tenant store and connection string resolver.
/// </summary>
public static class MultiTenancyHostingServiceCollectionExtensions
{
    /// <summary>
    /// Adds the full LHA multi-tenancy hosting stack:
    /// <list type="bullet">
    ///   <item>Core multi-tenancy services (via <c>AddLHAMultiTenancy</c>)</item>
    ///   <item><see cref="ConfigurationTenantStore"/> (replaces <c>NullTenantStore</c>)</item>
    ///   <item><see cref="MultiTenantConnectionStringResolver"/></item>
    /// </list>
    /// </summary>
    public static IServiceCollection AddLHAMultiTenancyHosting(this IServiceCollection services)
    {
        services.AddLHAMultiTenancy();

        // Replace NullTenantStore with configuration-backed store
        services.RemoveAll<ITenantStore>();
        services.TryAddSingleton<ITenantStore, ConfigurationTenantStore>();

        // Connection string resolver
        services.TryAddSingleton<MultiTenantConnectionStringResolver>();

        // Tenant Provisioning
        services.AddTransient<Provisioning.ITenantProvisionerStrategy, Provisioning.Strategies.SharedTenantProvisionerStrategy>();
        services.AddTransient<Provisioning.ITenantProvisionerStrategy, Provisioning.Strategies.PerTenantProvisionerStrategy>();
        services.AddTransient<Provisioning.ITenantProvisionerStrategy, Provisioning.Strategies.PerSchemaTenantProvisionerStrategy>();
        services.AddTransient<Provisioning.ITenantProvisionerStrategy, Provisioning.Strategies.HybridTenantProvisionerStrategy>();
        services.TryAddTransient<Provisioning.ITenantProvisioningOrchestrator, Provisioning.TenantProvisioningOrchestrator>();

        return services;
    }
}

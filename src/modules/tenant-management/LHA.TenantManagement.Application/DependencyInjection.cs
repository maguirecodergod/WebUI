using LHA.Ddd.Domain;
using LHA.TenantManagement.Application.Contracts;
using LHA.TenantManagement.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.TenantManagement.Application;

/// <summary>
/// Registers application-layer services for the Tenant Management module.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds <see cref="TenantAppService"/> and <see cref="TenantManager"/> to the container.
    /// </summary>
    public static IServiceCollection AddTenantManagementApplication(this IServiceCollection services)
    {
        services.TryAddScoped<ITenantAppService, TenantAppService>();
        services.TryAddTransient<TenantManager>();

        return services;
    }
}

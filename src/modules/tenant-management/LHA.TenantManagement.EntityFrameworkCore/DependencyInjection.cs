using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.TenantManagement.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.TenantManagement.EntityFrameworkCore;

/// <summary>
/// Registers EntityFrameworkCore infrastructure for the Tenant Management module.
/// </summary>
public static class TenantManagementEntityFrameworkCoreDependencyInjection
{
    /// <summary>
    /// Adds <see cref="TenantManagementDbContext"/>, the EF Core repository, and the
    /// database-backed <see cref="ITenantStore"/> to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">
    /// Optional EF Core <see cref="LhaDbContextOptions"/> configuration
    /// (e.g., to set the connection string or provider).
    /// </param>
    public static IServiceCollection AddTenantManagementEntityFrameworkCore(
        this IServiceCollection services,
        Action<LhaDbContextOptions>? configureOptions = null)
    {
        services.AddLhaDbContext<TenantManagementDbContext>(configureOptions);

        // Repository
        services.AddEfCoreRepository<TenantManagementDbContext, TenantEntity, Guid>();
        services.TryAddScoped<ITenantRepository, EfCoreTenantRepository>();

        // ITenantStore — replaces the default NullTenantStore with a DB-backed implementation
        services.Replace(ServiceDescriptor.Scoped<ITenantStore, EfCoreTenantStore>());

        return services;
    }
}

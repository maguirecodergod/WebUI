using LHA.EntityFrameworkCore;
using LHA.Identity.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.Identity.EntityFrameworkCore;

/// <summary>
/// Registers EntityFrameworkCore infrastructure for the Identity module.
/// </summary>
public static class IdentityEntityFrameworkCoreDependencyInjection
{
    /// <summary>
    /// Adds <see cref="IdentityDbContext"/>, all EF Core repositories.
    /// </summary>
    public static IServiceCollection AddIdentityEntityFrameworkCore(
        this IServiceCollection services,
        Action<LhaDbContextOptions>? configureOptions = null)
    {
        services.AddLhaDbContext<IdentityDbContext>(configureOptions);

        // Repositories
        services.AddEfCoreRepository<IdentityDbContext, IdentityUser, Guid>();
        services.AddEfCoreRepository<IdentityDbContext, IdentityRole, Guid>();
        services.AddEfCoreRepository<IdentityDbContext, IdentityClaimType, Guid>();
        services.AddEfCoreRepository<IdentityDbContext, IdentitySecurityLog, Guid>();
        services.AddEfCoreRepository<IdentityDbContext, IdentityPermissionGrant, Guid>();

        services.TryAddScoped<IIdentityUserRepository, EfCoreIdentityUserRepository>();
        services.TryAddScoped<IIdentityRoleRepository, EfCoreIdentityRoleRepository>();
        services.TryAddScoped<IIdentityClaimTypeRepository, EfCoreIdentityClaimTypeRepository>();
        services.TryAddScoped<IIdentitySecurityLogRepository, EfCoreIdentitySecurityLogRepository>();
        services.TryAddScoped<IPermissionGrantRepository, EfCorePermissionGrantRepository>();

        return services;
    }
}

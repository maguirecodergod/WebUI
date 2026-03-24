using LHA.Identity.Application.Contracts;
using LHA.Identity.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.Identity.Application;

/// <summary>
/// Registers application-layer services for the Identity module.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Identity application services, domain services, and infrastructure implementations.
    /// </summary>
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        // Infrastructure implementations
        services.TryAddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.TryAddSingleton<ILookupNormalizer, UpperInvariantLookupNormalizer>();
        services.TryAddSingleton<JwtTokenService>();

        // Domain services
        services.TryAddTransient<IdentityUserManager>();
        services.TryAddTransient<IdentityRoleManager>();

        // Application services
        services.TryAddScoped<IIdentityUserAppService, IdentityUserAppService>();
        services.TryAddScoped<IIdentityRoleAppService, IdentityRoleAppService>();
        services.TryAddScoped<IAuthAppService, AuthAppService>();
        services.TryAddScoped<IPermissionAppService, PermissionAppService>();
        services.TryAddScoped<IIdentityClaimTypeAppService, IdentityClaimTypeAppService>();
        services.TryAddScoped<IIdentitySecurityLogAppService, IdentitySecurityLogAppService>();

        // Cache User Lookup Factory
        services.AddSingleton<Func<Guid, CancellationToken, Task<LHA.Caching.CachedUserItem?>>>(sp =>
        {
            var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            return async (userId, ct) =>
            {
                using var scope = scopeFactory.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<LHA.Identity.Domain.IIdentityUserRepository>();
                var roleRepository = scope.ServiceProvider.GetRequiredService<LHA.Identity.Domain.IIdentityRoleRepository>();

                var user = await userRepository.FindAsync(userId, ct);
                if (user is null) return null;

                var roleIds = user.Roles.Select(r => r.RoleId).ToList();
                var roles = await roleRepository.GetListAsync(ct);
                var userRoles = roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToArray();

                return new LHA.Caching.CachedUserItem
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    TenantId = user.TenantId,
                    Roles = userRoles
                };
            };
        });

        return services;
    }
}

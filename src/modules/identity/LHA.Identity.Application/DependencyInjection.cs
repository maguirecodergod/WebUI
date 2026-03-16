using LHA.Ddd.Domain;
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

        return services;
    }
}

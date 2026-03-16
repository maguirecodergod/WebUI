using LHA.Account.Application.Contracts.Permissions;
using LHA.Account.Application.Permissions;
using LHA.AuditLog.Application;
using LHA.Identity.Application;
using LHA.PermissionManagement.Application;
using LHA.TenantManagement.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.Account.Application;

/// <summary>
/// Aggregates all module application-layer registrations for the Account Service.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers application services from Identity, TenantManagement,
    /// AuditLog, and PermissionManagement modules.
    /// </summary>
    public static IServiceCollection AddAccountApplication(this IServiceCollection services)
    {
        services.AddIdentityApplication();
        services.AddTenantManagementApplication();
        services.AddAuditLogApplication();
        services.AddPermissionManagementApplication();

        // Account-level services
        services.TryAddScoped<IPermissionRegistrationService, PermissionRegistrationService>();

        return services;
    }
}

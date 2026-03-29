using LHA.Account.Application.Contracts.Permissions;
using LHA.Account.Application.Permissions;
using LHA.Shared.Contracts.AuditLog;
using LHA.AuditLog.Application;
using LHA.Identity.Application;
using LHA.Identity.Domain;
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
        services.AddScoped<IAuditLogAppService, LHA.Account.Application.AuditLogs.AuditLogAppService>();

        // Bridge implementations for Identity module
        services.AddScoped<AccountUserTenantLookupService>();
        services.AddScoped<IUserTenantLookupService>(sp => sp.GetRequiredService<AccountUserTenantLookupService>());
        services.AddScoped<ITenantManagerBridge>(sp => sp.GetRequiredService<AccountUserTenantLookupService>());
        services.AddScoped<IPermissionStore, AccountPermissionStore>();

        // Tenant Provisioning Orchestration
        services.AddTransient<TenantProvisioning.ITenantProvisionerStrategy, TenantProvisioning.Strategies.SharedTenantProvisionerStrategy>();
        services.AddTransient<TenantProvisioning.ITenantProvisionerStrategy, TenantProvisioning.Strategies.PerTenantProvisionerStrategy>();
        services.AddTransient<TenantProvisioning.ITenantProvisionerStrategy, TenantProvisioning.Strategies.HybridTenantProvisionerStrategy>();
        services.AddTransient<TenantProvisioning.ITenantProvisioningOrchestrator, TenantProvisioning.TenantProvisioningOrchestrator>();

        return services;
    }
}

using LHA.AuditLog.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.AuditLog.Application;

/// <summary>
/// Registers application-layer services for the Audit Log module.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds <see cref="AuditLogAppService"/> to the container.
    /// </summary>
    public static IServiceCollection AddAuditLogApplication(this IServiceCollection services)
    {
        services.TryAddScoped<IAuditLogAppService, AuditLogAppService>();

        return services;
    }
}

using LHA.Auditing.Pipeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.AuditLog.EntityFrameworkCore;

/// <summary>
/// DI extensions for registering the EF Core audit log dispatcher.
/// </summary>
public static class EfCoreAuditLogServiceCollectionExtensions
{
    /// <summary>
    /// Registers the EF Core <see cref="IAuditLogDispatcher"/> as the primary
    /// audit log storage. Call after <c>AddLHAAuditPipeline()</c>.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configureDbContext">DbContext configuration delegate (e.g., UseNpgsql).</param>
    public static IServiceCollection AddLHAAuditEfCoreDispatcher(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureDbContext)
    {
        services.AddDbContext<AuditLogPipelineDbContext>(configureDbContext);

        // Replace the default logging dispatcher with EF Core
        services.AddSingleton<IAuditLogDispatcher, EfCoreAuditLogDispatcher>();

        return services;
    }
}

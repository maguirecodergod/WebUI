using LHA.Auditing;
using LHA.AuditLog.Domain;
using LHA.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.AuditLog.EntityFrameworkCore;

/// <summary>
/// Registers EntityFrameworkCore infrastructure for the Audit Log module.
/// </summary>
public static class AuditLogEntityFrameworkCoreDependencyInjection
{
    /// <summary>
    /// Adds <see cref="AuditLogDbContext"/>, the EF Core repository, and the
    /// database-backed <see cref="IAuditingStore"/> to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">
    /// Optional EF Core <see cref="LhaDbContextOptions"/> configuration
    /// (e.g., to set the connection string or provider).
    /// </param>
    public static IServiceCollection AddAuditLogEntityFrameworkCore(
        this IServiceCollection services,
        Action<LhaDbContextOptions>? configureOptions = null)
    {
        services.AddLhaDbContext<AuditLogDbContext>(configureOptions);

        // Repository
        services.AddEfCoreRepository<AuditLogDbContext, AuditLogEntity, Guid>();
        services.TryAddScoped<IAuditLogRepository, EfCoreAuditLogRepository>();

        // IAuditingStore — replaces the default LoggingAuditingStore with a DB-backed implementation
        services.Replace(ServiceDescriptor.Scoped<IAuditingStore, EfCoreAuditingStore>());

        return services;
    }
}

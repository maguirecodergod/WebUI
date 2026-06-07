using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.EntityChangeProperties;
using LHA.AuditLog.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.AuditLog.EntityFrameworkCore.PostgreSQL;

/// <summary>
/// Registers PostgreSQL-specific infrastructure for the Audit Log module.
/// This overrides the generic EF Core implementation with PostgreSQL-optimized features.
/// </summary>
public static class AuditLogPostgreSqlDependencyInjection
{
    /// <summary>
    /// Registers PostgreSQL-specific audit log infrastructure.
    /// Reads the Mode from the builder at registration time to conditionally register repositories.
    /// </summary>
    public static IServiceCollection AddAuditLogEntityFrameworkCorePostgreSQL(
        this IServiceCollection services,
        Action<AuditLogEntityFrameworkCoreBuilder>? configure = null)
    {
        var builder = new AuditLogEntityFrameworkCoreBuilder();
        builder.UsePostgreSql();
        configure?.Invoke(builder);

        AuditLogEntityFrameworkCoreDependencyInjection.Register(services, builder);

        RegisterRepositories(services, builder.Mode);

        return services;
    }

    private static void RegisterRepositories(
        IServiceCollection services,
        CAuditLogStoreMode mode)
    {
        services.RemoveAll<IAuditLogRepository>();
        services.RemoveAll<IAuditLogActionRepository>();
        services.RemoveAll<IEntityChangeRepository>();
        services.RemoveAll<IEntityPropertyChangeRepository>();
        services.RemoveAll<IAuditLogPipelineRepository>();

        if (mode.HasFlag(CAuditLogStoreMode.DataAudit))
        {
            services.TryAddScoped<IAuditLogRepository, EfCoreAuditLogPostgreSqlRepository>();
            services.TryAddScoped<IAuditLogActionRepository, EfCoreAuditLogActionPostgreSqlRepository>();
            services.TryAddScoped<IEntityChangeRepository, EfCoreEntityChangePostgreSqlRepository>();
            services.TryAddScoped<IEntityPropertyChangeRepository, EfCoreEntityPropertyChangePostgreSqlRepository>();
        }

        if (mode.HasFlag(CAuditLogStoreMode.Pipeline))
        {
            services.TryAddScoped<IAuditLogPipelineRepository, EfCoreAuditLogPipelinePostgreSqlRepository>();
        }
    }
}

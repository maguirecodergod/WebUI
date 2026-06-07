using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.EntityChangeProperties;
using LHA.AuditLog.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.AuditLog.EntityFrameworkCore.MongoDB;

/// <summary>
/// Registers MongoDB-specific infrastructure for the Audit Log module.
/// This overrides the generic EF Core implementation with MongoDB-appropriate handling.
/// </summary>
public static class AuditLogMongoDbDependencyInjection
{
    /// <summary>
    /// Registers MongoDB-specific audit log infrastructure.
    /// </summary>
    public static IServiceCollection AddAuditLogMongoDb(
        this IServiceCollection services,
        Action<AuditLogEntityFrameworkCoreBuilder>? configure = null)
    {
        var builder = new AuditLogEntityFrameworkCoreBuilder();
        builder.UseMongoDb();
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
            services.TryAddScoped<IAuditLogRepository, EfCoreAuditLogMongoDbRepository>();
            services.TryAddScoped<IAuditLogActionRepository, EfCoreAuditLogActionMongoDbRepository>();
            services.TryAddScoped<IEntityChangeRepository, EfCoreEntityChangeMongoDbRepository>();
            services.TryAddScoped<IEntityPropertyChangeRepository, EfCoreEntityPropertyChangeMongoDbRepository>();
        }

        if (mode.HasFlag(CAuditLogStoreMode.Pipeline))
        {
            services.TryAddScoped<IAuditLogPipelineRepository, EfCoreAuditLogPipelineMongoDbRepository>();
        }
    }
}

using LHA.AuditLog.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using LHA.AuditLog.EntityFrameworkCore.PostgreSQL.Configurations;

namespace LHA.AuditLog.EntityFrameworkCore.PostgreSQL;

/// <summary>
/// PostgreSQL specific model builder extensions for the Audit Log module.
/// </summary>
public static class AuditLogPostgreSqlModelBuilderExtensions
{
    /// <summary>
    /// Configures Audit Log entities with PostgreSQL-specific mapping (JSONB, Text, etc.).
    /// </summary>
    public static ModelBuilder ConfigureAuditLogPostgreSql(
        this ModelBuilder modelBuilder, 
        CAuditLogStoreMode mode = CAuditLogStoreMode.All)
    {
        // Global ignores
        modelBuilder.Ignore<LHA.Ddd.Domain.DomainEventRecord>();

        if (mode.HasFlag(CAuditLogStoreMode.DataAudit))
        {
            modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
            modelBuilder.ApplyConfiguration(new AuditLogActionConfiguration());
            modelBuilder.ApplyConfiguration(new EntityChangeConfiguration());
            modelBuilder.ApplyConfiguration(new EntityPropertyChangeConfiguration());
        }

        if (mode.HasFlag(CAuditLogStoreMode.Pipeline))
        {
            modelBuilder.ApplyConfiguration(new AuditLogPipelineConfiguration());
        }

        return modelBuilder;
    }

    /// <summary>
    /// Configures the Audit Log module to use PostgreSQL specific mappings.
    /// </summary>
    public static AuditLogEntityFrameworkCoreBuilder UsePostgreSql(this AuditLogEntityFrameworkCoreBuilder builder)
    {
        return builder.ConfigureModel(modelBuilder => 
        {
            modelBuilder.ConfigureAuditLogPostgreSql(builder.Mode);
        });
    }
}

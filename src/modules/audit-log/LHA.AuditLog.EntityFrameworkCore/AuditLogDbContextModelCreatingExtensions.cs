using LHA.AuditLog.EntityFrameworkCore.Configurations;
using Microsoft.EntityFrameworkCore;

namespace LHA.AuditLog.EntityFrameworkCore;

/// <summary>
/// EF Core model configuration for the Audit Log module entities.
/// </summary>
public static class AuditLogDbContextModelCreatingExtensions
{
    /// <summary>
    /// Configures all Audit Log entity types by applying their respective configurations.
    /// Can conditionally map models using <paramref name="mode"/>.
    /// </summary>
    public static void ConfigureAuditLog(this ModelBuilder modelBuilder, AuditLogStoreMode mode = AuditLogStoreMode.All)
    {
        // Global ignores
        modelBuilder.Ignore<LHA.Ddd.Domain.DomainEventRecord>();

        if (mode.HasFlag(AuditLogStoreMode.DataAudit))
        {
            modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
            modelBuilder.ApplyConfiguration(new AuditLogActionConfiguration());
            modelBuilder.ApplyConfiguration(new EntityChangeConfiguration());
            modelBuilder.ApplyConfiguration(new EntityPropertyChangeConfiguration());
        }
        else
        {
            modelBuilder.Ignore<LHA.AuditLog.Domain.AuditLogEntity>();
            modelBuilder.Ignore<LHA.AuditLog.Domain.AuditLogActionEntity>();
            modelBuilder.Ignore<LHA.AuditLog.Domain.EntityChangeEntity>();
            modelBuilder.Ignore<LHA.AuditLog.Domain.EntityPropertyChangeEntity>();
        }

        if (mode.HasFlag(AuditLogStoreMode.Pipeline))
        {
            modelBuilder.ApplyConfiguration(new AuditLogPipelineConfiguration());
        }
        else
        {
            modelBuilder.Ignore<LHA.AuditLog.Domain.AuditLogPipelineEntity>();
        }
    }
}

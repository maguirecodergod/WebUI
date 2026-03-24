using LHA.AuditLog.Domain;
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
    /// </summary>
    public static void ConfigureAuditLog(this ModelBuilder modelBuilder)
    {
        // Global ignores
        modelBuilder.Ignore<LHA.Ddd.Domain.DomainEventRecord>();

        // Apply entity configurations from the same assembly
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogActionConfiguration());
        modelBuilder.ApplyConfiguration(new EntityChangeConfiguration());
        modelBuilder.ApplyConfiguration(new EntityPropertyChangeConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogPipelineConfiguration());
    }
}

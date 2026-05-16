using LHA.AuditLog.EntityFrameworkCore;
using LHA.AuditLog.EntityFrameworkCore.MongoDB.Configurations;
using Microsoft.EntityFrameworkCore;

namespace LHA.AuditLog.EntityFrameworkCore.MongoDB;

/// <summary>
/// MongoDB specific model builder extensions for the Audit Log module.
/// </summary>
public static class AuditLogMongoDbModelBuilderExtensions
{
    /// <summary>
    /// Configures Audit Log entities with MongoDB-specific mapping (ToCollection, etc.).
    /// </summary>
    public static ModelBuilder ConfigureAuditLogMongoDb(
        this ModelBuilder modelBuilder, 
        AuditLogStoreMode mode = AuditLogStoreMode.All)
    {
        // Global ignores
        modelBuilder.Ignore<LHA.Ddd.Domain.DomainEventRecord>();

        if (mode.HasFlag(AuditLogStoreMode.DataAudit))
        {
            modelBuilder.ApplyConfiguration(new MongoDbAuditLogConfiguration());
        }

        if (mode.HasFlag(AuditLogStoreMode.Pipeline))
        {
            modelBuilder.ApplyConfiguration(new MongoDbAuditLogPipelineConfiguration());
        }

        return modelBuilder;
    }

    /// <summary>
    /// Configures the Audit Log module to use MongoDB specific mappings.
    /// </summary>
    public static AuditLogEntityFrameworkCoreBuilder UseMongoDb(this AuditLogEntityFrameworkCoreBuilder builder)
    {
        builder.SetAutoTransactionBehavior(AutoTransactionBehavior.Never);
        
        return builder.ConfigureModel(modelBuilder => 
        {
            modelBuilder.ConfigureAuditLogMongoDb(builder.Mode);
        });
    }
}

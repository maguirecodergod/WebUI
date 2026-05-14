using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LHA.Notification.Infrastructure.Persistences.Configurations;

/// <summary>
/// MongoDB-compatible configuration for <see cref="AuditLogActionEntity"/>.
/// </summary>
public sealed class AuditLogActionMongoConfiguration : IEntityTypeConfiguration<AuditLogActionEntity>
{
    public void Configure(EntityTypeBuilder<AuditLogActionEntity> b)
    {
        b.ToTable(DbSchemeConsts.Audit.Action);
        b.HasKey(e => e.Id);

        b.Property(e => e.ServiceName)
            .IsRequired()
            .HasMaxLength(AuditLogConsts.MaxServiceNameLength);

        b.Property(e => e.MethodName)
            .IsRequired()
            .HasMaxLength(AuditLogConsts.MaxMethodNameLength);

        b.Property(e => e.Parameters);

        b.HasIndex(e => e.AuditLogId);
        b.HasIndex(e => e.TenantId);
    }
}

using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LHA.Notification.Infrastructure.Persistences.Configurations;

/// <summary>
/// MongoDB-compatible configuration for <see cref="AuditLogEntity"/>.
/// Differs from the PostgreSQL version in <see cref="LHA.AuditLog.EntityFrameworkCore.Configurations"/>
/// by removing provider-specific type mappings (e.g. "jsonb") and cascade deletes.
/// </summary>
public sealed class AuditLogMongoConfiguration : IEntityTypeConfiguration<AuditLogEntity>
{
    public void Configure(EntityTypeBuilder<AuditLogEntity> b)
    {
        b.ToTable(DbSchemeConsts.Audit.Log);
        b.HasKey(e => e.Id);

        b.Property(e => e.ApplicationName)
            .HasMaxLength(AuditLogConsts.MaxApplicationNameLength);

        b.Property(e => e.ActionName)
            .HasMaxLength(AuditLogConsts.MaxActionNameLength);

        b.Property(e => e.RequestType)
            .HasConversion<byte>();

        b.Property(e => e.UserName)
            .HasMaxLength(AuditLogConsts.MaxUserNameLength);

        b.Property(e => e.TenantName)
            .HasMaxLength(AuditLogConsts.MaxTenantNameLength);

        b.Property(e => e.ClientId)
            .HasMaxLength(AuditLogConsts.MaxClientIdLength);

        b.Property(e => e.CorrelationId)
            .HasMaxLength(AuditLogConsts.MaxCorrelationIdLength);

        b.Property(e => e.ClientIpAddress)
            .HasMaxLength(AuditLogConsts.MaxClientIpAddressLength);

        b.Property(e => e.HttpMethod)
            .HasMaxLength(AuditLogConsts.MaxHttpMethodLength);

        b.Property(e => e.Url)
            .HasMaxLength(AuditLogConsts.MaxUrlLength);

        b.Property(e => e.BrowserInfo)
            .HasMaxLength(AuditLogConsts.MaxBrowserInfoLength);

        // MongoDB-compatible: use string instead of PostgreSQL "jsonb"
        b.Property(e => e.Exceptions);
        b.Property(e => e.Comments);
        b.Property(e => e.ExtraProperties);

        b.HasIndex(e => e.TenantId);
        b.HasIndex(e => e.UserId);
        b.HasIndex(e => e.ExecutionTime);
        b.HasIndex(e => e.RequestType);
        b.HasIndex(e => e.HttpStatusCode);
        b.HasIndex(e => e.CorrelationId);

        // Navigation: Actions stored as separate collection with manual FK
        b.HasMany(e => e.Actions)
            .WithOne()
            .HasForeignKey(a => a.AuditLogId);

        b.Navigation(e => e.Actions)
            .HasField("_actions")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Navigation: EntityChanges stored as separate collection with manual FK
        b.HasMany(e => e.EntityChanges)
            .WithOne()
            .HasForeignKey(c => c.AuditLogId);

        b.Navigation(e => e.EntityChanges)
            .HasField("_entityChanges")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LHA.Notification.Infrastructure.Persistences.Configurations;

/// <summary>
/// MongoDB-compatible configuration for <see cref="EntityChangeEntity"/>.
/// </summary>
public sealed class EntityChangeMongoConfiguration : IEntityTypeConfiguration<EntityChangeEntity>
{
    public void Configure(EntityTypeBuilder<EntityChangeEntity> b)
    {
        b.ToTable(DbSchemeConsts.Audit.EntityChange);
        b.HasKey(e => e.Id);

        b.Property(e => e.EntityId)
            .HasMaxLength(AuditLogConsts.MaxEntityIdLength);

        b.Property(e => e.EntityTypeFullName)
            .HasMaxLength(AuditLogConsts.MaxEntityTypeFullNameLength);

        b.HasIndex(e => e.AuditLogId);
        b.HasIndex(e => e.TenantId);
        b.HasIndex(e => new { e.EntityTypeFullName, e.EntityId });

        // Navigation: PropertyChanges stored as separate collection with manual FK
        b.HasMany(e => e.PropertyChanges)
            .WithOne()
            .HasForeignKey(p => p.EntityChangeId);

        b.Navigation(e => e.PropertyChanges)
            .HasField("_propertyChanges")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

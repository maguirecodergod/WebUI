using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LHA.Notification.Infrastructure.Persistences.Configurations;

/// <summary>
/// MongoDB-compatible configuration for <see cref="EntityPropertyChangeEntity"/>.
/// </summary>
public sealed class EntityPropertyChangeMongoConfiguration : IEntityTypeConfiguration<EntityPropertyChangeEntity>
{
    public void Configure(EntityTypeBuilder<EntityPropertyChangeEntity> b)
    {
        b.ToTable(DbSchemeConsts.Audit.PropertyChange);
        b.HasKey(e => e.Id);

        b.Property(e => e.PropertyName)
            .IsRequired()
            .HasMaxLength(AuditLogConsts.MaxPropertyNameLength);

        b.Property(e => e.PropertyTypeFullName)
            .IsRequired()
            .HasMaxLength(AuditLogConsts.MaxPropertyTypeFullNameLength);

        b.Property(e => e.OriginalValue)
            .HasMaxLength(AuditLogConsts.MaxPropertyValueLength);

        b.Property(e => e.NewValue)
            .HasMaxLength(AuditLogConsts.MaxPropertyValueLength);

        b.HasIndex(e => e.EntityChangeId);
    }
}

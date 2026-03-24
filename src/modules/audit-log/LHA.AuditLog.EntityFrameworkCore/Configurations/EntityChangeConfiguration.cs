using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LHA.AuditLog.EntityFrameworkCore.Configurations;

public sealed class EntityChangeConfiguration : IEntityTypeConfiguration<EntityChangeEntity>
{
    public void Configure(EntityTypeBuilder<EntityChangeEntity> b)
    {
        b.ToTable(AuditLogDbConsts.EntityChange);
        b.HasKey(e => e.Id);

        b.Property(e => e.EntityId)
            .HasMaxLength(AuditLogConsts.MaxEntityIdLength);

        b.Property(e => e.EntityTypeFullName)
            .HasMaxLength(AuditLogConsts.MaxEntityTypeFullNameLength);

        b.HasIndex(e => e.AuditLogId);
        b.HasIndex(e => e.TenantId);
        b.HasIndex(e => new { e.EntityTypeFullName, e.EntityId });

        b.HasMany(e => e.PropertyChanges)
            .WithOne()
            .HasForeignKey(p => p.EntityChangeId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Navigation(e => e.PropertyChanges)
            .HasField("_propertyChanges")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

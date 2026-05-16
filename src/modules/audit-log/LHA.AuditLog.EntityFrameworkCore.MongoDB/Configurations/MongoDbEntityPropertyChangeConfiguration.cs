using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;

namespace LHA.AuditLog.EntityFrameworkCore.MongoDB.Configurations;

public sealed class MongoDbEntityPropertyChangeConfiguration : IEntityTypeConfiguration<EntityPropertyChangeEntity>
{
    public void Configure(EntityTypeBuilder<EntityPropertyChangeEntity> b)
    {
        b.ToCollection(AuditLogDbConsts.EntityPropertyChange);
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
    }
}

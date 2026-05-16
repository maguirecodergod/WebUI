using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;

namespace LHA.AuditLog.EntityFrameworkCore.MongoDB.Configurations;

public sealed class MongoDbEntityChangeConfiguration : IEntityTypeConfiguration<EntityChangeEntity>
{
    public void Configure(EntityTypeBuilder<EntityChangeEntity> b)
    {
        b.ToCollection(AuditLogDbConsts.EntityChange);
        b.HasKey(e => e.Id);

        b.Property(e => e.EntityId)
            .HasMaxLength(AuditLogConsts.MaxEntityIdLength);

        b.Property(e => e.EntityTypeFullName)
            .HasMaxLength(AuditLogConsts.MaxEntityTypeFullNameLength);

        b.HasMany(e => e.PropertyChanges)
            .WithOne()
            .HasForeignKey(p => p.EntityChangeId);

        b.Navigation(e => e.PropertyChanges)
            .HasField("_propertyChanges")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

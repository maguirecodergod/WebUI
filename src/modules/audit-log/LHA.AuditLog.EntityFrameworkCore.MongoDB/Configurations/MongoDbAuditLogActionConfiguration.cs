using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;

namespace LHA.AuditLog.EntityFrameworkCore.MongoDB.Configurations;

public sealed class MongoDbAuditLogActionConfiguration : IEntityTypeConfiguration<AuditLogActionEntity>
{
    public void Configure(EntityTypeBuilder<AuditLogActionEntity> b)
    {
        b.ToCollection(AuditLogDbConsts.AuditLogAction);
        b.HasKey(e => e.Id);

        b.Property(e => e.ServiceName)
            .IsRequired()
            .HasMaxLength(AuditLogConsts.MaxServiceNameLength);

        b.Property(e => e.MethodName)
            .IsRequired()
            .HasMaxLength(AuditLogConsts.MaxMethodNameLength);

        // Remove relational column type
        b.Property(e => e.Parameters);
    }
}

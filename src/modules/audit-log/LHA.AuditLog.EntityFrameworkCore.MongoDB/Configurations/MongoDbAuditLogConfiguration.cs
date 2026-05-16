using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;

namespace LHA.AuditLog.EntityFrameworkCore.MongoDB.Configurations;

public sealed class MongoDbAuditLogConfiguration : IEntityTypeConfiguration<AuditLogEntity>
{
    public void Configure(EntityTypeBuilder<AuditLogEntity> b)
    {
        // MongoDB uses collections mapped via ToTable in this provider version
        b.ToCollection(AuditLogDbConsts.AuditLog);
        b.HasKey(e => e.Id);

        b.Property(e => e.ApplicationName)
            .HasMaxLength(AuditLogConsts.MaxApplicationNameLength);

        b.Property(e => e.ActionName)
            .HasMaxLength(AuditLogConsts.MaxActionNameLength);

        // MongoDB handles enums and byte conversion naturally, 
        // no need for relational column types like 'smallint'
        b.Property(e => e.RequestType);

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

        // Remove relational column types
        b.Property(e => e.Exceptions);
        b.Property(e => e.Comments);
        b.Property(e => e.ExtraProperties);

        // MongoDB handles relationships differently in EF Core.
        // Currently, EF Core MongoDB provider works best with owned entities or manual linking.
        // For simplicity and matching the relational structure:
        var actionsBuilder = b.OwnsMany(e => e.Actions);
        actionsBuilder.Property(x => x.ServiceName)
            .IsRequired()
            .HasMaxLength(AuditLogConsts.MaxServiceNameLength);
        actionsBuilder.Property(x => x.MethodName)
            .IsRequired()
            .HasMaxLength(AuditLogConsts.MaxMethodNameLength);
        actionsBuilder.Property(x => x.Parameters);


        b.Navigation(e => e.Actions)
            .HasField("_actions")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        var entityChangesBuilder = b.OwnsMany(e => e.EntityChanges);
        entityChangesBuilder.Property(x => x.EntityId)
            .HasMaxLength(AuditLogConsts.MaxEntityIdLength);
        entityChangesBuilder.Property(x => x.EntityTypeFullName)
            .HasMaxLength(AuditLogConsts.MaxEntityTypeFullNameLength);


        var propertyChangesBuilder = entityChangesBuilder.OwnsMany(x => x.PropertyChanges);
        propertyChangesBuilder.Property(x => x.PropertyName)
            .IsRequired()
            .HasMaxLength(AuditLogConsts.MaxPropertyNameLength);
        propertyChangesBuilder.Property(x => x.PropertyTypeFullName)
            .IsRequired()
            .HasMaxLength(AuditLogConsts.MaxPropertyTypeFullNameLength);
        propertyChangesBuilder.Property(x => x.OriginalValue)
            .HasMaxLength(AuditLogConsts.MaxPropertyValueLength);
        propertyChangesBuilder.Property(x => x.NewValue)
            .HasMaxLength(AuditLogConsts.MaxPropertyValueLength);


        b.Navigation(e => e.EntityChanges)
            .HasField("_entityChanges")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

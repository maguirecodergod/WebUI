using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LHA.AuditLog.EntityFrameworkCore.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLogEntity>
{
    public void Configure(EntityTypeBuilder<AuditLogEntity> b)
    {
        b.ToTable(AuditLogDbConsts.AuditLog);
        b.HasKey(e => e.Id);

        b.Property(e => e.ApplicationName)
            .HasMaxLength(AuditLogConsts.MaxApplicationNameLength);

        b.Property(e => e.ActionName)
            .HasMaxLength(AuditLogConsts.MaxActionNameLength);

        b.Property(e => e.RequestType)
            .HasConversion<byte>()
            .HasColumnType("smallint");

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

        b.Property(e => e.Exceptions).HasColumnType("text");
        b.Property(e => e.Comments).HasColumnType("text");
        b.Property(e => e.ExtraProperties).HasColumnType("text");

        b.HasIndex(e => e.TenantId);
        b.HasIndex(e => e.UserId);
        b.HasIndex(e => e.ExecutionTime);
        b.HasIndex(e => e.RequestType);
        b.HasIndex(e => e.HttpStatusCode);
        b.HasIndex(e => e.CorrelationId);

        b.HasMany(e => e.Actions)
            .WithOne()
            .HasForeignKey(a => a.AuditLogId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Navigation(e => e.Actions)
            .HasField("_actions")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        b.HasMany(e => e.EntityChanges)
            .WithOne()
            .HasForeignKey(c => c.AuditLogId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Navigation(e => e.EntityChanges)
            .HasField("_entityChanges")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

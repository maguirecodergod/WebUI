using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace LHA.AuditLog.EntityFrameworkCore;

/// <summary>
/// EF Core model configuration for the Audit Log module entities.
/// </summary>
public static class AuditLogDbContextModelCreatingExtensions
{
    /// <summary>
    /// Configures all Audit Log entity types with table names, columns, indices, and relationships.
    /// </summary>
    public static void ConfigureAuditLog(this ModelBuilder modelBuilder)
    {
        // AuditLog does not use DomainEventRecord
        modelBuilder.Ignore<LHA.Ddd.Domain.DomainEventRecord>();

        modelBuilder.Entity<AuditLogEntity>(b =>
        {
            b.ToTable("AuditLogs");
            b.HasKey(e => e.Id);

            b.Property(e => e.ApplicationName)
                .HasMaxLength(AuditLogConsts.MaxApplicationNameLength);

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

            // Exceptions, Comments, ExtraProperties — unbounded text
            b.Property(e => e.Exceptions).HasColumnType("text");
            b.Property(e => e.Comments).HasColumnType("text");
            b.Property(e => e.ExtraProperties).HasColumnType("text");

            // Indices for common query patterns
            b.HasIndex(e => e.TenantId);
            b.HasIndex(e => e.UserId);
            b.HasIndex(e => e.ExecutionTime);
            b.HasIndex(e => e.HttpStatusCode);
            b.HasIndex(e => e.CorrelationId);

            // Navigation: Actions
            b.HasMany(e => e.Actions)
                .WithOne()
                .HasForeignKey(a => a.AuditLogId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Navigation(e => e.Actions)
                .HasField("_actions")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // Navigation: EntityChanges
            b.HasMany(e => e.EntityChanges)
                .WithOne()
                .HasForeignKey(c => c.AuditLogId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Navigation(e => e.EntityChanges)
                .HasField("_entityChanges")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<AuditLogActionEntity>(b =>
        {
            b.ToTable("AuditLogActions");
            b.HasKey(e => e.Id);

            b.Property(e => e.ServiceName)
                .IsRequired()
                .HasMaxLength(AuditLogConsts.MaxServiceNameLength);

            b.Property(e => e.MethodName)
                .IsRequired()
                .HasMaxLength(AuditLogConsts.MaxMethodNameLength);

            b.Property(e => e.Parameters).HasColumnType("text");

            b.HasIndex(e => e.AuditLogId);
            b.HasIndex(e => e.TenantId);
        });

        modelBuilder.Entity<EntityChangeEntity>(b =>
        {
            b.ToTable("EntityChanges");
            b.HasKey(e => e.Id);

            b.Property(e => e.EntityId)
                .HasMaxLength(AuditLogConsts.MaxEntityIdLength);

            b.Property(e => e.EntityTypeFullName)
                .HasMaxLength(AuditLogConsts.MaxEntityTypeFullNameLength);

            b.HasIndex(e => e.AuditLogId);
            b.HasIndex(e => e.TenantId);
            b.HasIndex(e => new { e.EntityTypeFullName, e.EntityId });

            // Navigation: PropertyChanges
            b.HasMany(e => e.PropertyChanges)
                .WithOne()
                .HasForeignKey(p => p.EntityChangeId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Navigation(e => e.PropertyChanges)
                .HasField("_propertyChanges")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<EntityPropertyChangeEntity>(b =>
        {
            b.ToTable("EntityPropertyChanges");
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
        });
    }
}

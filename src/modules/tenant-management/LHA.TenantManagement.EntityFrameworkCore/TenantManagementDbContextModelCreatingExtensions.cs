using LHA.Core;
using LHA.EntityFrameworkCore;
using LHA.TenantManagement.Domain;
using LHA.TenantManagement.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace LHA.TenantManagement.EntityFrameworkCore;

/// <summary>
/// EF Core model configuration for the Tenant Management module entities.
/// </summary>
public static class TenantManagementDbContextModelCreatingExtensions
{
    /// <summary>
    /// Configures the <see cref="TenantEntity"/> and <see cref="TenantConnectionString"/> entity types
    /// with proper columns, indices, constraints, and conventions.
    /// </summary>
    public static void ConfigureTenantManagement(this ModelBuilder modelBuilder)
    {
        // DomainEventRecord is not a mapped entity — it's the in-memory event wrapper on AggregateRoot.
        modelBuilder.Ignore<LHA.Ddd.Domain.DomainEventRecord>();

        modelBuilder.Entity<TenantEntity>(b =>
        {
            b.ToTable("Tenants");
            b.ConfigureByConvention(); // audit, soft-delete, concurrency stamp, etc.

            b.HasKey(t => t.Id);

            b.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(TenantConsts.MaxNameLength);

            b.Property(t => t.NormalizedName)
                .IsRequired()
                .HasMaxLength(TenantConsts.MaxNameLength);

            b.Property(t => t.Status)
                .IsRequired()
                .HasDefaultValue(CMasterStatus.Active)
                .HasSentinel((CMasterStatus)0);

            b.Property(t => t.DatabaseStyle)
                .IsRequired()
                .HasDefaultValue(MultiTenancyDatabaseStyle.Shared)
                .HasSentinel((MultiTenancyDatabaseStyle)0);

            b.HasIndex(t => t.NormalizedName).IsUnique();

            b.HasMany(t => t.ConnectionStrings)
                .WithOne()
                .HasForeignKey(cs => cs.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            // EF Core reads/writes the private backing field directly
            b.Navigation(t => t.ConnectionStrings)
                .HasField("_connectionStrings")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<TenantConnectionString>(b =>
        {
            b.ToTable("TenantConnectionStrings");
            b.HasKey(cs => cs.Id);

            b.Property(cs => cs.Name)
                .IsRequired()
                .HasMaxLength(TenantConsts.MaxConnectionStringNameLength);

            b.Property(cs => cs.Value)
                .IsRequired()
                .HasMaxLength(TenantConsts.MaxConnectionStringValueLength);

            b.HasIndex(cs => new { cs.TenantId, cs.Name }).IsUnique();
        });
    }
}

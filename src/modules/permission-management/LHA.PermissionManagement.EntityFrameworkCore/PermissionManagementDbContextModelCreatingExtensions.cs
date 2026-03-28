using LHA.EntityFrameworkCore;
using LHA.PermissionManagement.Domain;
using LHA.PermissionManagement.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace LHA.PermissionManagement.EntityFrameworkCore;

public static class PermissionManagementDbContextModelCreatingExtensions
{
    public static void ConfigurePermissionManagement(this ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<LHA.Ddd.Domain.DomainEventRecord>();

        // ─── PermissionDefinition ────────────────────────────────
        modelBuilder.Entity<PermissionDefinitionEntity>(b =>
        {
            b.ToTable("PermissionDefinitions");
            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(PermissionDefinitionConsts.MaxNameLength);

            b.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(PermissionDefinitionConsts.MaxDisplayNameLength);

            b.Property(x => x.ServiceName)
                .IsRequired()
                .HasMaxLength(PermissionDefinitionConsts.MaxServiceNameLength);

            b.Property(x => x.GroupName)
                .HasMaxLength(PermissionDefinitionConsts.MaxGroupNameLength);

            b.Property(x => x.Description)
                .HasMaxLength(PermissionDefinitionConsts.MaxDescriptionLength);

            b.HasIndex(x => x.Name).IsUnique();
            b.HasIndex(x => x.ServiceName);
        });

        // ─── PermissionGroup ─────────────────────────────────────
        modelBuilder.Entity<PermissionGroupEntity>(b =>
        {
            b.ToTable("PermissionGroups");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(PermissionGroupConsts.MaxNameLength);

            b.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(PermissionGroupConsts.MaxDisplayNameLength);

            b.Property(x => x.ServiceName)
                .IsRequired()
                .HasMaxLength(PermissionGroupConsts.MaxServiceNameLength);

            b.Property(x => x.Description)
                .HasMaxLength(PermissionGroupConsts.MaxDescriptionLength);

            b.HasIndex(x => x.Name).IsUnique();

            b.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(i => i.PermissionGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Navigation(x => x.Items)
                .HasField("_items")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        // ─── PermissionGroupItem ─────────────────────────────────
        modelBuilder.Entity<PermissionGroupItemEntity>(b =>
        {
            b.ToTable("PermissionGroupItems");
            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.PermissionGroupId, x.PermissionDefinitionId }).IsUnique();
        });

        // ─── PermissionTemplate ──────────────────────────────────
        modelBuilder.Entity<PermissionTemplateEntity>(b =>
        {
            b.ToTable("PermissionTemplates");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(PermissionTemplateConsts.MaxNameLength);

            b.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(PermissionTemplateConsts.MaxDisplayNameLength);

            b.Property(x => x.Description)
                .HasMaxLength(PermissionTemplateConsts.MaxDescriptionLength);

            b.HasIndex(x => x.Name).IsUnique();

            b.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(i => i.PermissionTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Navigation(x => x.Items)
                .HasField("_items")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        // ─── PermissionTemplateItem ──────────────────────────────
        modelBuilder.Entity<PermissionTemplateItemEntity>(b =>
        {
            b.ToTable("PermissionTemplateItems");
            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.PermissionTemplateId, x.PermissionGroupId }).IsUnique();
        });

        // ─── PermissionGrant ─────────────────────────────────────
        modelBuilder.Entity<PermissionGrantEntity>(b =>
        {
            b.ToTable("PermissionGrants");
            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(PermissionDefinitionConsts.MaxNameLength);

            b.Property(x => x.ProviderName)
                .IsRequired()
                .HasMaxLength(PermissionGrantConsts.MaxProviderNameLength);

            b.Property(x => x.ProviderKey)
                .IsRequired()
                .HasMaxLength(PermissionGrantConsts.MaxProviderKeyLength);

            b.HasIndex(x => new { x.Name, x.ProviderName, x.ProviderKey }).IsUnique();
            b.HasIndex(x => new { x.ProviderName, x.ProviderKey });
            b.HasIndex(x => x.TenantId);
        });
    }
}

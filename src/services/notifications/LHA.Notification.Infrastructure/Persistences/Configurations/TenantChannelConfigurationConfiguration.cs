using LHA.Notification.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LHA.Notification.Infrastructure.Persistences.Configurations;

public sealed class TenantChannelConfigurationConfiguration : IEntityTypeConfiguration<ChannelConfigurationEntity>
{
    public void Configure(EntityTypeBuilder<ChannelConfigurationEntity> b)
    {
        b.ToTable(DbSchemeConsts.Notification.TenantChannelConfigs);
        b.HasKey(e => e.Id);

        b.Property(e => e.Channel)
            .HasConversion<byte>()
            .HasColumnType("smallint");

        b.Property(e => e.ProviderType)
            .HasConversion<byte>()
            .HasColumnType("smallint");

        b.Property(e => e.SettingsJson)
            .HasColumnType("text");

        b.Property(e => e.IsEnabled).IsRequired().HasDefaultValue(true);

        b.HasIndex(e => e.TenantId);
        b.HasIndex(e => new { e.TenantId, e.Channel }).IsUnique();
    }
}

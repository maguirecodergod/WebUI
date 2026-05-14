using LHA.Notification.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LHA.Notification.Infrastructure.Persistences.Configurations;

public sealed class TenantChannelConfigurationConfiguration : IEntityTypeConfiguration<TenantChannelConfigurationEntity>
{
    public void Configure(EntityTypeBuilder<TenantChannelConfigurationEntity> b)
    {
        b.ToTable(DbSchemeConsts.Notification.TenantChannelConfigs);
        b.HasKey(e => e.Id);

        b.Property(e => e.Channel)
            .HasConversion<byte>()
            .HasColumnType("smallint");

        b.Property(e => e.ProviderType)
            .HasConversion<byte>()
            .HasColumnType("smallint");

        b.Property(e => e.ServiceAccountJson)
            .HasMaxLength(NotificationDbConsts.MaxServiceAccountLength);

        b.Property(e => e.ApiKey)
            .HasMaxLength(NotificationDbConsts.MaxApiKeyLength);

        b.Property(e => e.Host)
            .HasMaxLength(NotificationDbConsts.MaxHostLength);

        b.Property(e => e.Port);

        b.Property(e => e.Username)
            .HasMaxLength(NotificationDbConsts.MaxUsernameLength);

        b.Property(e => e.Password)
            .HasMaxLength(NotificationDbConsts.MaxUsernameLength);

        b.Property(e => e.UseSsl).IsRequired().HasDefaultValue(true);

        b.Property(e => e.CustomSettings)
            .HasColumnType("text");

        b.Property(e => e.IsEnabled).IsRequired().HasDefaultValue(true);

        b.HasIndex(e => e.TenantId);
        b.HasIndex(e => new { e.TenantId, e.Channel }).IsUnique();
    }
}

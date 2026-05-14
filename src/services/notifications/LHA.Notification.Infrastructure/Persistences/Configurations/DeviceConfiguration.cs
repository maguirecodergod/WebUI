using LHA.Notification.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LHA.Notification.Infrastructure.Persistences.Configurations;

public sealed class DeviceConfiguration : IEntityTypeConfiguration<DeviceEntity>
{
    public void Configure(EntityTypeBuilder<DeviceEntity> b)
    {
        b.ToTable(DbSchemeConsts.Notification.Devices);
        b.HasKey(e => e.Id);

        b.Property(e => e.UserId).IsRequired();

        b.Property(e => e.Platform)
            .HasConversion<byte>()
            .HasColumnType("smallint");

        b.Property(e => e.Token)
            .IsRequired()
            .HasMaxLength(NotificationDbConsts.MaxTokenLength);

        b.Property(e => e.TokenHash)
            .IsRequired()
            .HasMaxLength(NotificationDbConsts.MaxHashLength);

        b.Property(e => e.AppVersion)
            .IsRequired()
            .HasMaxLength(NotificationDbConsts.MaxVersionLength);

        b.Property(e => e.OsVersion)
            .IsRequired()
            .HasMaxLength(NotificationDbConsts.MaxVersionLength);

        b.Property(e => e.DeviceModel)
            .IsRequired()
            .HasMaxLength(NotificationDbConsts.MaxDeviceModelLength);

        b.Property(e => e.Locale)
            .IsRequired()
            .HasMaxLength(NotificationDbConsts.MaxLocaleLength);

        b.Property(e => e.Timezone)
            .IsRequired()
            .HasMaxLength(NotificationDbConsts.MaxTimezoneLength);

        b.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

        b.Property(e => e.LastSeenAt).IsRequired();
        b.Property(e => e.RegisteredAt).IsRequired();

        b.HasIndex(e => e.TenantId);
        b.HasIndex(e => e.UserId);
        b.HasIndex(e => e.TokenHash).IsUnique();
        b.HasIndex(e => e.Platform);
        b.HasIndex(e => e.IsActive);
    }
}

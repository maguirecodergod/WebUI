using LHA.Notification.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LHA.Notification.Infrastructure.Persistences.Configurations;

public sealed class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreferenceEntity>
{
    public void Configure(EntityTypeBuilder<UserPreferenceEntity> b)
    {
        b.ToTable(DbSchemeConsts.Notification.UserPreferences);
        b.HasKey(e => e.Id);

        b.Property(e => e.UserId).IsRequired();

        b.Property(e => e.GlobalOptOut).IsRequired().HasDefaultValue(false);

        b.Property(e => e.Timezone)
            .IsRequired()
            .HasMaxLength(NotificationDbConsts.MaxTimezoneLength)
            .HasDefaultValue("UTC");

        b.Property(e => e.Locale)
            .IsRequired()
            .HasMaxLength(NotificationDbConsts.MaxLocaleLength)
            .HasDefaultValue("en");

        b.OwnsMany(e => e.Channels, channel =>
        {
            channel.WithOwner().HasForeignKey("UserPreferenceId");
            channel.Property<Guid>("Id");
            channel.HasKey("Id");

            channel.Property(c => c.Channel)
                .HasConversion<byte>()
                .HasColumnType("smallint");

            channel.Property(c => c.Enabled).IsRequired().HasDefaultValue(true);

            channel.Property(c => c.Categories)
                .HasColumnType("text");
        });

        b.OwnsMany(e => e.Categories, category =>
        {
            category.WithOwner().HasForeignKey("UserPreferenceId");
            category.Property<Guid>("Id");
            category.HasKey("Id");

            category.Property(c => c.Category)
                .IsRequired()
                .HasMaxLength(NotificationDbConsts.MaxCodeLength);

            category.Property(c => c.Enabled).IsRequired().HasDefaultValue(true);

            category.Property(c => c.Channels)
                .HasColumnType("text");
        });

        b.OwnsOne(e => e.QuietHours, quietHours =>
        {
            quietHours.Property(q => q.Enabled).IsRequired().HasDefaultValue(false);
            quietHours.Property(q => q.StartTime);
            quietHours.Property(q => q.EndTime);
            quietHours.Property(q => q.Timezone)
                .IsRequired()
                .HasMaxLength(NotificationDbConsts.MaxTimezoneLength);
        });

        b.HasIndex(e => e.TenantId);
        b.HasIndex(e => e.UserId).IsUnique();
    }
}

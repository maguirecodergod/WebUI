using System.Text.Json;
using LHA.Notification.Domain;
using LHA.Notification.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LHA.Notification.Infrastructure.Persistences.Configurations;

public sealed class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreferenceEntity>
{
    private static readonly JsonSerializerOptions JsonOptions = new();

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
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, JsonOptions) ?? new List<string>())
                .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
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
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonOptions),
                    v => JsonSerializer.Deserialize<List<CNotificationChannel>>(v, JsonOptions) ?? new List<CNotificationChannel>())
                .Metadata.SetValueComparer(new ValueComparer<List<CNotificationChannel>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
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

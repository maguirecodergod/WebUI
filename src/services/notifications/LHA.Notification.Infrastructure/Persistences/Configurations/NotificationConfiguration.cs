using LHA.Notification.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LHA.Notification.Infrastructure.Persistences.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<NotificationEntity>
{
    public void Configure(EntityTypeBuilder<NotificationEntity> b)
    {
        b.ToTable(DbSchemeConsts.Notification.Notifications);
        b.HasKey(e => e.Id);

        b.Property(e => e.CorrelationId)
            .IsRequired()
            .HasMaxLength(NotificationDbConsts.MaxCorrelationIdLength);

        b.Property(e => e.BatchId);

        b.Property(e => e.RecipientId).IsRequired();

        b.Property(e => e.RecipientType)
            .HasConversion<byte>()
            .HasColumnType("smallint");

        b.Property(e => e.Type)
            .HasConversion<byte>()
            .HasColumnType("smallint");

        b.Property(e => e.Priority)
            .HasConversion<byte>()
            .HasColumnType("smallint");

        b.Property(e => e.Status)
            .HasConversion<byte>()
            .HasColumnType("smallint");

        b.Property(e => e.Subject)
            .HasMaxLength(NotificationDbConsts.MaxSubjectLength);

        b.Property(e => e.Body)
            .IsRequired()
            .HasMaxLength(NotificationDbConsts.MaxBodyLength);

        b.Property(e => e.Data)
            .HasColumnType("text");

        b.Property(e => e.ImageUrl)
            .HasMaxLength(NotificationDbConsts.MaxUrlLength);

        b.Property(e => e.ActionUrl)
            .HasMaxLength(NotificationDbConsts.MaxUrlLength);

        b.Property(e => e.TemplateId);

        b.Property(e => e.TemplateVariables)
            .HasColumnType("text");

        b.Property(e => e.ScheduledAt);
        b.Property(e => e.ExpiresAt);
        b.Property(e => e.SentAt);
        b.Property(e => e.DeliveredAt);
        b.Property(e => e.ReadAt);
        b.Property(e => e.FailedAt);

        b.Property(e => e.RetryCount).IsRequired().HasDefaultValue(0);
        b.Property(e => e.MaxRetries).IsRequired().HasDefaultValue(3);

        b.OwnsMany(e => e.Channels, channel =>
        {
            channel.WithOwner().HasForeignKey("NotificationId");
            channel.Property<Guid>("Id");
            channel.HasKey("Id");

            channel.Property(c => c.Channel)
                .HasConversion<byte>()
                .HasColumnType("smallint");

            channel.Property(c => c.Status)
                .HasConversion<byte>()
                .HasColumnType("smallint");

            channel.Property(c => c.ProviderType)
                .HasConversion<byte>()
                .HasColumnType("smallint");

            channel.Property(c => c.ExternalId)
                .HasMaxLength(NotificationDbConsts.MaxExternalIdLength);

            channel.Property(c => c.SentAt);
            channel.Property(c => c.DeliveredAt);
            channel.Property(c => c.FailedAt);

            channel.Property(c => c.FailureReason)
                .HasMaxLength(NotificationDbConsts.MaxFailureReasonLength);

            channel.Property(c => c.RetryCount).IsRequired().HasDefaultValue(0);

            channel.Property(c => c.Metadata)
                .HasColumnType("text");
        });

        b.HasIndex(e => e.TenantId);
        b.HasIndex(e => e.RecipientId);
        b.HasIndex(e => e.Status);
        b.HasIndex(e => e.CorrelationId).IsUnique();
        b.HasIndex(e => e.CreationTime);
    }
}

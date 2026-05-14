using LHA.Notification.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LHA.Notification.Infrastructure.Persistences.Configurations;

public sealed class NotificationBatchConfiguration : IEntityTypeConfiguration<NotificationBatchEntity>
{
    public void Configure(EntityTypeBuilder<NotificationBatchEntity> b)
    {
        b.ToTable(DbSchemeConsts.Notification.NotificationBatches);
        b.HasKey(e => e.Id);

        b.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(NotificationDbConsts.MaxNameLength);

        b.Property(e => e.Status)
            .HasConversion<byte>()
            .HasColumnType("smallint");

        b.Property(e => e.TotalCount).IsRequired().HasDefaultValue(0L);
        b.Property(e => e.SentCount).IsRequired().HasDefaultValue(0L);
        b.Property(e => e.DeliveredCount).IsRequired().HasDefaultValue(0L);
        b.Property(e => e.FailedCount).IsRequired().HasDefaultValue(0L);
        b.Property(e => e.PendingCount).IsRequired().HasDefaultValue(0L);

        b.Property(e => e.TemplateId)
            .HasMaxLength(NotificationDbConsts.MaxCodeLength);

        b.Property(e => e.ScheduledAt);
        b.Property(e => e.StartedAt);
        b.Property(e => e.CompletedAt);

        b.HasIndex(e => e.TenantId);
        b.HasIndex(e => e.Status);
        b.HasIndex(e => e.ScheduledAt);
    }
}

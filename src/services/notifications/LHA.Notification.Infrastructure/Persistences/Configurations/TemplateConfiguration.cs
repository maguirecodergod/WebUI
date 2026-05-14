using LHA.Notification.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LHA.Notification.Infrastructure.Persistences.Configurations;

public sealed class TemplateConfiguration : IEntityTypeConfiguration<TemplateEntity>
{
    public void Configure(EntityTypeBuilder<TemplateEntity> b)
    {
        b.ToTable(DbSchemeConsts.Notification.Templates);
        b.HasKey(e => e.Id);

        b.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(NotificationDbConsts.MaxCodeLength);

        b.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(NotificationDbConsts.MaxNameLength);

        b.Property(e => e.Description)
            .HasMaxLength(NotificationDbConsts.MaxDescriptionLength);

        b.Property(e => e.Type)
            .HasConversion<byte>()
            .HasColumnType("smallint");

        b.Property(e => e.SupportedChannels)
            .HasColumnType("text");

        b.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        b.Property(e => e.IsSystem).IsRequired().HasDefaultValue(false);

        b.Property(e => e.DefaultLocale)
            .IsRequired()
            .HasMaxLength(NotificationDbConsts.MaxLocaleLength)
            .HasDefaultValue("en");

        b.Property(e => e.Tags)
            .HasColumnType("text");

        b.OwnsMany(e => e.Versions, version =>
        {
            version.WithOwner().HasForeignKey("TemplateId");
            version.Property<Guid>("Id");
            version.HasKey("Id");

            version.Property(v => v.Version).IsRequired();
            version.Property(v => v.Locale)
                .IsRequired()
                .HasMaxLength(NotificationDbConsts.MaxLocaleLength);

            version.Property(v => v.Channel)
                .HasConversion<byte>()
                .HasColumnType("smallint");

            version.Property(v => v.SubjectTemplate)
                .HasMaxLength(NotificationDbConsts.MaxSubjectLength);

            version.Property(v => v.BodyTemplate)
                .IsRequired()
                .HasMaxLength(NotificationDbConsts.MaxTemplateLength);

            version.Property(v => v.HtmlTemplate)
                .HasMaxLength(NotificationDbConsts.MaxTemplateLength);

            version.Property(v => v.PushTitle)
                .HasMaxLength(NotificationDbConsts.MaxSubjectLength);

            version.Property(v => v.PushBody)
                .HasMaxLength(NotificationDbConsts.MaxBodyLength);

            version.Property(v => v.IsDefault).IsRequired().HasDefaultValue(false);
            version.Property(v => v.CreatedAt).IsRequired();

            version.OwnsMany(v => v.Variables, variable =>
            {
                variable.WithOwner().HasForeignKey("TemplateVersionId");
                variable.Property<Guid>("Id");
                variable.HasKey("Id");

                variable.Property(v => v.Name)
                    .IsRequired()
                    .HasMaxLength(NotificationDbConsts.MaxCodeLength);

                variable.Property(v => v.Type)
                    .HasConversion<byte>()
                    .HasColumnType("smallint");

                variable.Property(v => v.Required).IsRequired().HasDefaultValue(false);

                variable.Property(v => v.DefaultValue)
                    .HasMaxLength(NotificationDbConsts.MaxNameLength);

                variable.Property(v => v.Description)
                    .HasMaxLength(NotificationDbConsts.MaxDescriptionLength);
            });
        });

        b.HasIndex(e => e.TenantId);
        b.HasIndex(e => e.Code).IsUnique();
        b.HasIndex(e => e.Type);
        b.HasIndex(e => e.IsActive);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LHA.Tax.Domain;
using LHA.Core;
using LHA.Tax.Domain.Aggregates;

namespace LHA.Tax.EntityFrameworkCore.Configurations;

public sealed class TaxJurisdictionEntityConfiguration : IEntityTypeConfiguration<TaxJurisdictionEntity>
{
    public void Configure(EntityTypeBuilder<TaxJurisdictionEntity> b)
    {
        b.ToTable(DbSchemeConsts.Tax.Jurisdiction);
        b.HasKey(e => e.Id);

        b.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxJurisdictionCodeLength);

        b.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxNameLength);

        b.Property(e => e.Level)
            .HasConversion<byte>()
            .HasColumnType("smallint");

        b.Property(e => e.PrimaryRegime)
            .HasConversion<byte>()
            .HasColumnType("tinyint");

        b.Property(e => e.ParentJurisdictionId).IsRequired();

        b.Property(e => e.LocalCurrencyCode)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxLocalCurrencyCodeLength);

        b.Property(e => e.Timezone)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxTimezoneLength);

        b.Property(e => e.IsCompoundWithParent).IsRequired().HasDefaultValue(false);

        b.Property(e => e.Status).IsRequired().HasDefaultValue((byte)CMasterStatus.Active);

        b.HasOne(e => e.ParentJurisdiction)
            .WithMany(e => e.Children)
            .HasForeignKey(e => e.ParentJurisdictionId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(e => e.TaxRegimes)
            .WithOne(e => e.Jurisdiction)
            .HasForeignKey(e => e.JurisdictionId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(e => e.Code).IsUnique();
        b.HasIndex(e => e.ParentJurisdictionId);
        b.HasIndex(e => e.Level);
        b.HasIndex(e => e.Status);
    }
}

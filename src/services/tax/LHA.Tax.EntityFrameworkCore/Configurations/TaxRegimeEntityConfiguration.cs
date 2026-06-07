using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LHA.Tax.Domain;
using LHA.Tax.Domain.Configuration;

namespace LHA.Tax.EntityFrameworkCore.Configurations;

public sealed class TaxRegimeEntityConfiguration : IEntityTypeConfiguration<TaxRegimeEntity>
{
    public void Configure(EntityTypeBuilder<TaxRegimeEntity> b)
    {
        b.ToTable(DbSchemeConsts.Tax.Regime);
        b.HasKey(e => e.Id);

        b.Property(e => e.JurisdictionId).IsRequired();

        b.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxNameLength);

        b.Property(e => e.LocalName)
            .HasMaxLength(TaxDbConsts.MaxLocalNameLength);

        b.Property(e => e.RegimeType)
            .HasConversion<byte>()
            .HasColumnType("tinyint");

        b.Property(e => e.InvoiceCode)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxCodeLength);

        b.Property(e => e.RegistrationNumberFormat)
            .HasMaxLength(TaxDbConsts.MaxRegistrationNumberFormatLength);

        b.Property(e => e.VatVerificationApiUrl)
            .HasMaxLength(TaxDbConsts.MaxUrlLength);

        b.Property(e => e.IsTaxInclusive).IsRequired().HasDefaultValue(false);

        b.Property(e => e.IsCascading).IsRequired().HasDefaultValue(false);

        b.Property(e => e.FilingFrequency)
            .HasMaxLength(TaxDbConsts.MaxFilingFrequencyLength);

        b.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

        b.HasOne(e => e.Jurisdiction)
            .WithMany(j => j.TaxRegimes)
            .HasForeignKey(e => e.JurisdictionId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(e => e.Rates)
            .WithOne(e => e.TaxRegime)
            .HasForeignKey(e => e.TaxRegimeId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(e => e.JurisdictionId);
        b.HasIndex(e => e.InvoiceCode);
        b.HasIndex(e => e.IsActive);
    }
}

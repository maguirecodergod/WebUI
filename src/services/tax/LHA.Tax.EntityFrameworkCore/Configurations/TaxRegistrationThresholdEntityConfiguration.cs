using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LHA.Tax.Domain;
using LHA.Tax.Domain.Configuration;

namespace LHA.Tax.EntityFrameworkCore.Configurations;

public sealed class TaxRegistrationThresholdEntityConfiguration : IEntityTypeConfiguration<TaxRegistrationThresholdEntity>
{
    public void Configure(EntityTypeBuilder<TaxRegistrationThresholdEntity> b)
    {
        b.ToTable(DbSchemeConsts.Tax.RegistrationThreshold);
        b.HasKey(e => e.Id);

        b.Property(e => e.JurisdictionId).IsRequired();

        b.Property(e => e.ThresholdAmount).IsRequired();

        b.Property(e => e.CurrencyCode)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxCurrencyCodeLength);

        b.Property(e => e.MeasurementPeriod)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxMeasurementPeriodLength)
            .HasDefaultValue("Rolling12Months");

        b.Property(e => e.TransactionCountThreshold);

        b.Property(e => e.DigitalServicesOnly).IsRequired().HasDefaultValue(false);

        b.Property(e => e.EffectiveFrom).IsRequired();

        b.Property(e => e.EffectiveTo).IsRequired();

        b.HasOne(e => e.Jurisdiction)
            .WithMany(t => t.Thresholds)
            .HasForeignKey(e => e.JurisdictionId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(e => e.JurisdictionId);
        b.HasIndex(e => e.EffectiveFrom);
        b.HasIndex(e => e.EffectiveTo);
    }
}

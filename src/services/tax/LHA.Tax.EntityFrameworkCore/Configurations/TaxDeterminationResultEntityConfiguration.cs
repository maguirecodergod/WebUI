using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LHA.Tax.Domain;
using LHA.Tax.Domain.Determination;

namespace LHA.Tax.EntityFrameworkCore.Configurations;

public sealed class TaxDeterminationResultEntityConfiguration : IEntityTypeConfiguration<TaxDeterminationResultEntity>
{
    public void Configure(EntityTypeBuilder<TaxDeterminationResultEntity> b)
    {
        b.ToTable(DbSchemeConsts.Tax.DeterminationResult);
        b.HasKey(e => e.Id);

        b.Property(e => e.RequestId).IsRequired();

        b.Property(e => e.AppliedTaxRateId);

        b.Property(e => e.RateCategory)
            .HasConversion<byte>()
            .HasColumnType("tinyint");

        b.Property(e => e.LiabilityType)
            .HasConversion<byte>()
            .HasColumnType("tinyint");

        b.Property(e => e.PlaceOfSupplyRuleApplied)
            .HasConversion<byte>()
            .HasColumnType("tinyint");

        b.Property(e => e.TaxingJurisdictionId).IsRequired();

        b.Property(e => e.TaxRate).IsRequired();

        b.Property(e => e.TaxableAmount).IsRequired();

        b.Property(e => e.TaxAmountInBillingCurrency).IsRequired();

        b.Property(e => e.TaxAmountInLocalCurrency).IsRequired();

        b.Property(e => e.AppliedExemptionId);

        b.Property(e => e.ReasonCode)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxReasonCodeLength);

        b.Property(e => e.ReasonNarrative)
            .HasMaxLength(TaxDbConsts.MaxReasonNarrativeLength);

        b.Property(e => e.DeterminedAt).IsRequired();

        b.Property(e => e.TaxEngineVersion)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxEngineVersionLength);

        b.HasOne(e => e.AppliedTaxRate)
            .WithMany()
            .HasForeignKey(e => e.AppliedTaxRateId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(e => e.TaxingJurisdiction)
            .WithMany()
            .HasForeignKey(e => e.TaxingJurisdictionId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.AppliedExemption)
            .WithMany()
            .HasForeignKey(e => e.AppliedExemptionId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasIndex(e => e.RequestId);
        b.HasIndex(e => e.TaxingJurisdictionId);
        b.HasIndex(e => e.DeterminedAt);
    }
}

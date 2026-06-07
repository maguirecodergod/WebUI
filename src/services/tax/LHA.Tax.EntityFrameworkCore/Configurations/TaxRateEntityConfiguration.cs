using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LHA.Tax.Domain;
using LHA.Core;
using LHA.Tax.Domain.Configuration;

namespace LHA.Tax.EntityFrameworkCore.Configurations;

public sealed class TaxRateEntityConfiguration : IEntityTypeConfiguration<TaxRateEntity>
{
    public void Configure(EntityTypeBuilder<TaxRateEntity> b)
    {
        b.ToTable(DbSchemeConsts.Tax.Rate);
        b.HasKey(e => e.Id);

        b.Property(e => e.TaxRegimeId).IsRequired();

        b.Property(e => e.TaxProductCategoryId);

        b.Property(e => e.RateCategory)
            .HasConversion<byte>()
            .HasColumnType("tinyint");

        b.Property(e => e.ApplicableToCustomerStatus)
            .HasConversion<byte>()
            .HasColumnType("smallint");

        b.Property(e => e.Rate).IsRequired();

        b.Property(e => e.EffectiveFrom).IsRequired();

        b.Property(e => e.EffectiveTo).IsRequired();

        b.Property(e => e.LegalReference)
            .HasMaxLength(TaxDbConsts.MaxLegalReferenceLength);

        b.Property(e => e.Notes)
            .HasMaxLength(TaxDbConsts.MaxNotesLength);

        b.Property(e => e.Status)
            .HasConversion<byte>()
            .HasColumnType("tinyint")
            .HasDefaultValue((byte)CMasterStatus.Active);

        b.HasOne(e => e.TaxRegime)
            .WithMany(r => r.Rates)
            .HasForeignKey(e => e.TaxRegimeId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.TaxProductCategory)
            .WithMany()
            .HasForeignKey(e => e.TaxProductCategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasIndex(e => e.TaxRegimeId);
        b.HasIndex(e => e.TaxProductCategoryId);
        b.HasIndex(e => e.RateCategory);
        b.HasIndex(e => e.EffectiveFrom);
        b.HasIndex(e => e.EffectiveTo);
        b.HasIndex(e => e.Status);
    }
}

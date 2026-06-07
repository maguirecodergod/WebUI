using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LHA.Tax.Domain;
using LHA.Tax.Domain.Determination;

namespace LHA.Tax.EntityFrameworkCore.Configurations;

public sealed class TaxDeterminationRequestEntityConfiguration : IEntityTypeConfiguration<TaxDeterminationRequestEntity>
{
    public void Configure(EntityTypeBuilder<TaxDeterminationRequestEntity> b)
    {
        b.ToTable(DbSchemeConsts.Tax.DeterminationRequest);
        b.HasKey(e => e.Id);

        b.Property(e => e.TaxPointDate).IsRequired();

        b.Property(e => e.SupplierJurisdictionId).IsRequired();

        b.Property(e => e.CustomerJurisdictionId).IsRequired();

        b.Property(e => e.CustomerDeliveryJurisdictionId);

        b.Property(e => e.TaxProductCategoryId).IsRequired();

        b.Property(e => e.CustomerTaxProfileId).IsRequired();

        b.Property(e => e.LineAmountExcludingTax).IsRequired();

        b.Property(e => e.BillingCurrencyCode)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxCurrencyCodeLength);

        b.Property(e => e.ExchangeRateToLocalCurrency).IsRequired();

        b.HasIndex(e => e.TaxPointDate);
        b.HasIndex(e => e.SupplierJurisdictionId);
        b.HasIndex(e => e.CustomerJurisdictionId);
        b.HasIndex(e => e.TaxProductCategoryId);
    }
}

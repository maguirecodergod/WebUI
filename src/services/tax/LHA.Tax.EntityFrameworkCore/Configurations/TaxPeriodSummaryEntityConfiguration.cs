using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LHA.Tax.Domain;
using LHA.Tax.Domain.Reporting;

namespace LHA.Tax.EntityFrameworkCore.Configurations;

public sealed class TaxPeriodSummaryEntityConfiguration : IEntityTypeConfiguration<TaxPeriodSummaryEntity>
{
    public void Configure(EntityTypeBuilder<TaxPeriodSummaryEntity> b)
    {
        b.ToTable(DbSchemeConsts.Tax.PeriodSummary);
        b.HasKey(e => e.Id);

        b.Property(e => e.BusinessTaxRegistrationId).IsRequired();

        b.Property(e => e.PeriodStartDate).IsRequired();

        b.Property(e => e.PeriodEndDate).IsRequired();

        b.Property(e => e.ReportingCurrencyCode)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxCurrencyCodeLength);

        // Output tax
        b.Property(e => e.StandardRatedSales).IsRequired();
        b.Property(e => e.StandardRatedTaxCollected).IsRequired();

        b.Property(e => e.ReducedRatedSales).IsRequired();
        b.Property(e => e.ReducedRatedTaxCollected).IsRequired();

        b.Property(e => e.ZeroRatedSales).IsRequired();
        b.Property(e => e.ExemptSales).IsRequired();
        b.Property(e => e.ReverseChargeSales).IsRequired();

        // Input tax
        b.Property(e => e.InputTaxRecoverable).IsRequired();
        b.Property(e => e.InputTaxNonRecoverable).IsRequired();

        // Net
        b.Property(e => e.NetTaxPayable).IsRequired();

        b.Property(e => e.ReturnStatus)
            .HasConversion<byte>()
            .HasColumnType("tinyint");

        b.Property(e => e.SubmittedAt);

        b.Property(e => e.PaidAt);

        b.Property(e => e.AuthorityReferenceNumber)
            .HasMaxLength(TaxDbConsts.MaxAuthorityReferenceLength);

        b.HasOne(e => e.BusinessTaxRegistration)
            .WithMany()
            .HasForeignKey(e => e.BusinessTaxRegistrationId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(e => e.BusinessTaxRegistrationId);
        b.HasIndex(e => e.PeriodStartDate);
        b.HasIndex(e => e.PeriodEndDate);
        b.HasIndex(e => e.ReturnStatus);
    }
}

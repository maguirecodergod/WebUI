using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LHA.Tax.Domain;
using LHA.Tax.Domain.Invoicing;

namespace LHA.Tax.EntityFrameworkCore.Configurations;

public sealed class InvoiceTaxLineEntityConfiguration : IEntityTypeConfiguration<InvoiceTaxLineEntity>
{
    public void Configure(EntityTypeBuilder<InvoiceTaxLineEntity> b)
    {
        b.ToTable(DbSchemeConsts.Tax.InvoiceTaxLine);
        b.HasKey(e => e.Id);

        b.Property(e => e.InvoiceId).IsRequired();

        b.Property(e => e.InvoiceLineId).IsRequired();

        b.Property(e => e.TaxDeterminationResultId).IsRequired();

        b.Property(e => e.TaxRegimeId).IsRequired();

        b.Property(e => e.DisplayLabel)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxDisplayLabelLength);

        b.Property(e => e.AppliedRate).IsRequired();

        b.Property(e => e.TaxableAmount).IsRequired();

        b.Property(e => e.TaxAmount).IsRequired();

        b.Property(e => e.CurrencyCode)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxCurrencyCodeLength);

        b.Property(e => e.CustomerVatNumberOnInvoice)
            .HasMaxLength(TaxDbConsts.MaxCustomerVatNumberLength);

        b.Property(e => e.LegalNoteForInvoice)
            .HasMaxLength(TaxDbConsts.MaxLegalNoteLength);

        b.Property(e => e.SequenceOrder).IsRequired().HasDefaultValue(1);

        b.HasOne(e => e.TaxDeterminationResult)
            .WithMany()
            .HasForeignKey(e => e.TaxDeterminationResultId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.TaxRegime)
            .WithMany()
            .HasForeignKey(e => e.TaxRegimeId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(e => e.InvoiceId);
        b.HasIndex(e => e.InvoiceLineId);
        b.HasIndex(e => e.TaxDeterminationResultId);
        b.HasIndex(e => e.TaxRegimeId);
    }
}

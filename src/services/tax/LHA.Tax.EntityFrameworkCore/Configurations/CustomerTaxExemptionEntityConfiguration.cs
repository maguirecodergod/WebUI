using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LHA.Tax.Domain;
using LHA.Core;
using LHA.Tax.Domain.Customers;
using LHA.Tax.Domain.Configuration;

namespace LHA.Tax.EntityFrameworkCore.Configurations;

public sealed class CustomerTaxExemptionEntityConfiguration : IEntityTypeConfiguration<CustomerTaxExemptionEntity>
{
    public void Configure(EntityTypeBuilder<CustomerTaxExemptionEntity> b)
    {
        b.ToTable(DbSchemeConsts.Tax.CustomerTaxExemption);
        b.HasKey(e => e.Id);

        b.Property(e => e.CustomerTaxProfileId).IsRequired();

        b.Property(e => e.JurisdictionId).IsRequired();

        // Many-to-many for applicable categories
        b.HasMany(e => e.ApplicableCategories)
            .WithMany()
            .UsingEntity(
                "Tax_CategoryExemption",
                l => l.HasOne(typeof(TaxProductCategoryEntity))
                    .WithMany()
                    .HasForeignKey("CategoryId")
                    .OnDelete(DeleteBehavior.Restrict),
                r => r.HasOne(typeof(CustomerTaxExemptionEntity))
                    .WithMany("ApplicableCategories")
                    .HasForeignKey("ExemptionId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("CategoryId", "ExemptionId");
                    j.HasIndex("CategoryId");
                    j.HasIndex("ExemptionId");
                });

        b.Property(e => e.ExemptionType)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxExemptionTypeLength);

        b.Property(e => e.CertificateNumber)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxCertificateNumberLength);

        b.Property(e => e.DocumentStorageKey)
            .HasMaxLength(TaxDbConsts.MaxDocumentStorageKeyLength);

        b.Property(e => e.IssuedDate).IsRequired();

        b.Property(e => e.ExpiryDate).IsRequired();

        b.Property(e => e.Status)
            .HasConversion<byte>()
            .HasColumnType("tinyint")
            .HasDefaultValue((byte)CMasterStatus.Active);

        b.HasOne(e => e.CustomerTaxProfile)
            .WithMany(e => e.Exemptions)
            .HasForeignKey(e => e.CustomerTaxProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(e => e.Jurisdiction)
            .WithMany()
            .HasForeignKey(e => e.JurisdictionId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(e => e.CustomerTaxProfileId);
        b.HasIndex(e => e.JurisdictionId);
        b.HasIndex(e => e.ExemptionType);
        b.HasIndex(e => e.Status);
    }
}

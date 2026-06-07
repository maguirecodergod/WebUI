using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LHA.Tax.Domain;
using LHA.Tax.Domain.Customers;

namespace LHA.Tax.EntityFrameworkCore.Configurations;

public sealed class CustomerTaxIdentifierEntityConfiguration : IEntityTypeConfiguration<CustomerTaxIdentifierEntity>
{
    public void Configure(EntityTypeBuilder<CustomerTaxIdentifierEntity> b)
    {
        b.ToTable(DbSchemeConsts.Tax.CustomerTaxIdentifier);
        b.HasKey(e => e.Id);

        b.Property(e => e.CustomerTaxProfileId).IsRequired();

        b.Property(e => e.JurisdictionId).IsRequired();

        b.Property(e => e.TaxRegistrationNumber)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxRegistrationNumberLength);

        b.Property(e => e.VerificationStatus)
            .HasConversion<byte>()
            .HasColumnType("tinyint");

        b.Property(e => e.LastVerifiedAt);

        b.Property(e => e.VerificationResponse)
            .HasMaxLength(TaxDbConsts.MaxVerificationResponseLength);

        b.Property(e => e.ValidFrom).IsRequired();

        b.Property(e => e.ValidTo).IsRequired();

        b.HasOne(e => e.CustomerTaxProfile)
            .WithMany(t => t.TaxIdentifiers)
            .HasForeignKey(e => e.CustomerTaxProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(e => e.Jurisdiction)
            .WithMany()
            .HasForeignKey(e => e.JurisdictionId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(e => e.CustomerTaxProfileId);
        b.HasIndex(e => e.JurisdictionId);
        b.HasIndex(e => e.VerificationStatus);
        b.HasIndex(e => e.ValidFrom);
        b.HasIndex(e => e.ValidTo);
    }
}

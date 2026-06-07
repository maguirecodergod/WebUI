using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LHA.Tax.Domain;
using LHA.Tax.Domain.Customers;

namespace LHA.Tax.EntityFrameworkCore.Configurations;

public sealed class CustomerTaxProfileEntityConfiguration : IEntityTypeConfiguration<CustomerTaxProfileEntity>
{
    public void Configure(EntityTypeBuilder<CustomerTaxProfileEntity> b)
    {
        b.ToTable(DbSchemeConsts.Tax.CustomerTaxProfile);
        b.HasKey(e => e.Id);

        b.Property(e => e.CustomerId).IsRequired();

        b.Property(e => e.TaxStatus)
            .HasConversion<byte>()
            .HasColumnType("tinyint");

        b.Property(e => e.EstablishedJurisdictionId).IsRequired();

        b.Property(e => e.IsEuConsumer).IsRequired().HasDefaultValue(false);

        b.HasOne(e => e.EstablishedJurisdiction)
            .WithMany()
            .HasForeignKey(e => e.EstablishedJurisdictionId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(e => e.TaxIdentifiers)
            .WithOne(e => e.CustomerTaxProfile)
            .HasForeignKey(e => e.CustomerTaxProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(e => e.Exemptions)
            .WithOne(e => e.CustomerTaxProfile)
            .HasForeignKey(e => e.CustomerTaxProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(e => e.CustomerId);
        b.HasIndex(e => e.EstablishedJurisdictionId);
        b.HasIndex(e => e.TaxStatus);
    }
}

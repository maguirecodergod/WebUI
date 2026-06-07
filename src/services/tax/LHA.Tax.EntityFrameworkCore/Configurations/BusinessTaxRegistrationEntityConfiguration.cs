using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LHA.Core;
using LHA.Tax.Domain.BusinessRegistrations;

namespace LHA.Tax.EntityFrameworkCore.Configurations;

public sealed class BusinessTaxRegistrationEntityConfiguration : IEntityTypeConfiguration<BusinessTaxRegistrationEntity>
{
    public void Configure(EntityTypeBuilder<BusinessTaxRegistrationEntity> b)
    {
        b.ToTable(DbSchemeConsts.Tax.BusinessTaxRegistration);
        b.HasKey(e => e.Id);

        b.Property(e => e.TaxRegimeId).IsRequired();

        b.Property(e => e.RegistrationNumber)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxRegistrationNumberLength);

        b.Property(e => e.RegistrationDate).IsRequired();

        b.Property(e => e.DeregistrationDate);

        b.Property(e => e.IsOssRegistration).IsRequired().HasDefaultValue(false);

        b.Property(e => e.LegalEntityName)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxLegalEntityNameLength);

        b.Property(e => e.LegalEntityAddress)
            .HasMaxLength(TaxDbConsts.MaxLegalEntityAddressLength);

        b.Property(e => e.Status)
            .HasConversion<byte>()
            .HasColumnType("tinyint")
            .HasDefaultValue((byte)CMasterStatus.Active);

        b.HasOne(e => e.TaxRegime)
            .WithMany(r => r.BusinessRegistrations)
            .HasForeignKey(e => e.TaxRegimeId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(e => e.TaxRegimeId);
        b.HasIndex(e => e.RegistrationNumber);
        b.HasIndex(e => e.IsOssRegistration);
        b.HasIndex(e => e.Status);
    }
}

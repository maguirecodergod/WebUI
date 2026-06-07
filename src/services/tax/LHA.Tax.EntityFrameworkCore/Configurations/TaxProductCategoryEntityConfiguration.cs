using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LHA.Tax.Domain;
using LHA.Tax.Domain.Configuration;

namespace LHA.Tax.EntityFrameworkCore.Configurations;

public sealed class TaxProductCategoryEntityConfiguration : IEntityTypeConfiguration<TaxProductCategoryEntity>
{
    public void Configure(EntityTypeBuilder<TaxProductCategoryEntity> b)
    {
        b.ToTable(DbSchemeConsts.Tax.ProductCategory);
        b.HasKey(e => e.Id);

        b.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxCodeLength);

        b.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(TaxDbConsts.MaxNameLength);

        b.Property(e => e.Description)
            .HasMaxLength(TaxDbConsts.MaxDescriptionLength);

        b.Property(e => e.HsCode)
            .HasMaxLength(TaxDbConsts.MaxHsCodeLength);

        b.Property(e => e.EuCnCode)
            .HasMaxLength(TaxDbConsts.MaxEuCnCodeLength);

        b.Property(e => e.IsDigitalService).IsRequired().HasDefaultValue(false);

        b.Property(e => e.IsService).IsRequired().HasDefaultValue(false);

        b.HasMany(e => e.Rates)
            .WithOne(e => e.TaxProductCategory)
            .HasForeignKey(e => e.TaxProductCategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasIndex(e => e.Code).IsUnique();
        b.HasIndex(e => e.IsDigitalService);
        b.HasIndex(e => e.IsService);
    }
}

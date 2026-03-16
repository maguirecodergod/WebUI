using LHA.Ddd.Domain;
using LHA.EntityFrameworkCore;
using LHA.Mega.Domain.Account;
using Microsoft.EntityFrameworkCore;

namespace LHA.Mega.EntityFrameworkCore;

public static class MegaDbContextModelCreatingExtensions
{
    public static void ConfigureMega(this ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<DomainEventRecord>();

        modelBuilder.Entity<MegaAccountEntity>(b =>
        {
            b.ToTable("Mega_Account");
            b.ConfigureByConvention();
            b.HasKey(a => a.Id);

            b.Property(a => a.Code).IsRequired().HasMaxLength(50);
            b.Property(a => a.Name).IsRequired().HasMaxLength(256);
            b.Property(a => a.PhoneNumber).HasMaxLength(20);
            b.Property(a => a.Email).HasMaxLength(256);
            b.Property(a => a.Address).HasMaxLength(500);
            b.Property(a => a.IsActive).IsRequired().HasDefaultValue(true);

            b.HasIndex(a => new { a.TenantId, a.Code }).IsUnique();
        });
    }
}

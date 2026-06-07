using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LHA.Tax.EntityFrameworkCore
{
    /// <summary>
    /// Design-time factory used by <c>dotnet ef migrations</c>.
    /// Provides an <see cref="TaxDbContextFactory"/> configured with Npgsql
    /// for migration scaffolding.
    /// </summary>
    public sealed class TaxDbContextFactory : IDesignTimeDbContextFactory<TaxDbContext>
    {
        public TaxDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TaxDbContext>();
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=LienHoaApp_Tax;Username=admin;Password=admin");

            return new TaxDbContext(optionsBuilder.Options);
        }
    }
}
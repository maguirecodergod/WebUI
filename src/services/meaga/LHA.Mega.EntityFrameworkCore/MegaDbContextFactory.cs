using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LHA.Mega.EntityFrameworkCore
{
    /// <summary>
    /// Design-time factory used by <c>dotnet ef migrations</c>.
    /// Provides an <see cref="MegaDbContext"/> configured with Npgsql
    /// for migration scaffolding.
    /// </summary>
    public sealed class MegaDbContextFactory
        : IDesignTimeDbContextFactory<MegaDbContext>
    {
        public MegaDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MegaDbContext>();
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=LienHoaApp_Mega;Username=admin;Password=admin");

            return new MegaDbContext(optionsBuilder.Options);
        }
    }
}
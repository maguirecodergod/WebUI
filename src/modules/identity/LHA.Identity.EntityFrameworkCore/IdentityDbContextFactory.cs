using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LHA.Identity.EntityFrameworkCore;

/// <summary>
/// Design-time factory used by <c>dotnet ef migrations</c>.
/// Provides an <see cref="IdentityDbContext"/> configured with Npgsql
/// for migration scaffolding.
/// </summary>
public sealed class IdentityDbContextFactory
    : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=LienHoaApp_Identity;Username=admin;Password=admin");

        return new IdentityDbContext(optionsBuilder.Options);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LHA.Account.EntityFrameworkCore;

/// <summary>
/// Design-time factory used by <c>dotnet ef migrations</c>.
/// Provides an <see cref="AccountDbContext"/> configured with Npgsql
/// for migration scaffolding.
/// </summary>
public sealed class AccountDbContextFactory
    : IDesignTimeDbContextFactory<AccountDbContext>
{
    public AccountDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AccountDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5433;Database=LienHoaApp_Account;Username=postgres;Password=Khuong@090217");

        return new AccountDbContext(optionsBuilder.Options);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LHA.TenantManagement.EntityFrameworkCore;

/// <summary>
/// Design-time factory used by <c>dotnet ef migrations</c>.
/// Provides a <see cref="TenantManagementDbContext"/> configured with Npgsql
/// for migration scaffolding.
/// </summary>
public sealed class TenantManagementDbContextFactory
    : IDesignTimeDbContextFactory<TenantManagementDbContext>
{
    public TenantManagementDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TenantManagementDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=LienHoaApp_TenantManagement;Username=postgres;Password=Khuong@090217");

        return new TenantManagementDbContext(optionsBuilder.Options);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LHA.AuditLog.EntityFrameworkCore;

/// <summary>
/// Design-time factory used by <c>dotnet ef migrations</c>.
/// Provides an <see cref="AuditLogDbContext"/> configured with Npgsql
/// for migration scaffolding.
/// </summary>
public sealed class AuditLogDbContextFactory
    : IDesignTimeDbContextFactory<AuditLogDbContext>
{
    public AuditLogDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuditLogDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5433;Database=LienHoaApp_AuditLog;Username=postgres;Password=Khuong@090217");

        return new AuditLogDbContext(optionsBuilder.Options);
    }
}

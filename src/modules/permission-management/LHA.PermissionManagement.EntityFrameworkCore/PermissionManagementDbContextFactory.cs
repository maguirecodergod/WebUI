using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LHA.PermissionManagement.EntityFrameworkCore;

public sealed class PermissionManagementDbContextFactory
    : IDesignTimeDbContextFactory<PermissionManagementDbContext>
{
    public PermissionManagementDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PermissionManagementDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=LienHoaApp_PermissionManagement;Username=admin;Password=admin");

        return new PermissionManagementDbContext(optionsBuilder.Options);
    }
}

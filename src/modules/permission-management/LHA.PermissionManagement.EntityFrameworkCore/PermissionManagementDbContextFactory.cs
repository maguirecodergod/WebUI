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
            "Host=localhost;Port=5432;Database=LienHoaApp_PermissionManagement;Username=postgres;Password=Khuong@090217");

        return new PermissionManagementDbContext(optionsBuilder.Options);
    }
}

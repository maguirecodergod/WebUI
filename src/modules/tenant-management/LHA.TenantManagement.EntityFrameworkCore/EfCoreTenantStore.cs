using LHA.MultiTenancy;
using LHA.TenantManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.TenantManagement.EntityFrameworkCore;

/// <summary>
/// EF Core–based <see cref="ITenantStore"/> that reads tenant configuration directly
/// from <see cref="TenantManagementDbContext"/>.
/// <para>
/// Uses <see cref="IServiceScopeFactory"/> to create an independent scope, because
/// tenant resolution happens in middleware <b>before</b> a Unit of Work is started.
/// This avoids the "no active unit of work" error from <c>UnitOfWorkDbContextProvider</c>.
/// </para>
/// </summary>
public sealed class EfCoreTenantStore(IServiceScopeFactory scopeFactory) : ITenantStore
{
    /// <inheritdoc />
    public async Task<TenantConfiguration?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TenantManagementDbContext>();

        var tenant = await dbContext.Tenants
            .AsNoTracking()
            .Include(t => t.ConnectionStrings)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return tenant is not null ? MapToConfiguration(tenant) : null;
    }

    /// <inheritdoc />
    public async Task<TenantConfiguration?> FindByNameAsync(
        string normalizedName,
        CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TenantManagementDbContext>();

        var tenant = await dbContext.Tenants
            .AsNoTracking()
            .Include(t => t.ConnectionStrings)
            .FirstOrDefaultAsync(t => t.NormalizedName == normalizedName, cancellationToken);

        return tenant is not null ? MapToConfiguration(tenant) : null;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TenantConfiguration>> GetListAsync(
        CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TenantManagementDbContext>();

        var tenants = await dbContext.Tenants
            .AsNoTracking()
            .Include(t => t.ConnectionStrings)
            .Where(t => t.Status == LHA.Core.CMasterStatus.Active)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        return tenants.ConvertAll(MapToConfiguration);
    }

    // ─── Mapping ─────────────────────────────────────────────────────

    private static TenantConfiguration MapToConfiguration(TenantEntity tenant) => new()
    {
        Id = tenant.Id,
        Name = tenant.Name,
        NormalizedName = tenant.NormalizedName,
        Status = tenant.Status,
        ConnectionStrings = tenant.ConnectionStrings
            .ToDictionary(cs => cs.Name, cs => cs.Value)
    };
}

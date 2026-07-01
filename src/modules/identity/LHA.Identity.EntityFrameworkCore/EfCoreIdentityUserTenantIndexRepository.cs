using LHA.Ddd.Domain;
using LHA.EntityFrameworkCore;
using LHA.Identity.Domain;
using LHA.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace LHA.Identity.EntityFrameworkCore;

/// <summary>
/// EF Core implementation of <see cref="IUserTenantIndexRepository"/>.
/// </summary>
public sealed class EfCoreIdentityUserTenantIndexRepository
    : EfCoreRepository<IdentityDbContext, IdentityUserTenantIndex, Guid>, IUserTenantIndexRepository
{
    private readonly ICurrentTenant _currentTenant;

    public EfCoreIdentityUserTenantIndexRepository(
        IDbContextProvider<IdentityDbContext> dbContextProvider,
        ICurrentTenant currentTenant)
        : base(dbContextProvider)
    {
        _currentTenant = currentTenant;
    }

    /// <inheritdoc />
    public async Task<Guid?> FindTenantIdByNormalizedUserNameAsync(
        string normalizedUserName,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var index = await dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.NormalizedUserName == normalizedUserName, cancellationToken);
        return index?.TenantId;
    }

    /// <inheritdoc />
    public async Task<Guid?> FindTenantIdByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var index = await dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
        return index?.TenantId;
    }

    /// <inheritdoc />
    public async Task<(Guid UserId, Guid? TenantId)?> FindUserAndTenantAsync(
        string normalizedUserNameOrEmail,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        // Try username first
        var byUserName = await dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.NormalizedUserName == normalizedUserNameOrEmail, cancellationToken);

        if (byUserName is not null)
            return (byUserName.UserId, byUserName.TenantId);

        // Try email
        var byEmail = await dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedUserNameOrEmail, cancellationToken);

        return byEmail is not null ? (byEmail.UserId, byEmail.TenantId) : null;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(
        string normalizedUserName,
        string normalizedEmail,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AsNoTracking()
            .AnyAsync(x =>
                x.NormalizedUserName == normalizedUserName ||
                x.NormalizedEmail == normalizedEmail,
                cancellationToken);
    }
}

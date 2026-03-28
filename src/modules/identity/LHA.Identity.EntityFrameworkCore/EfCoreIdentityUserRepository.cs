using LHA.Core;
using LHA.Ddd.Domain;
using LHA.EntityFrameworkCore;
using LHA.Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace LHA.Identity.EntityFrameworkCore;

/// <summary>
/// EF Core implementation of <see cref="IIdentityUserRepository"/>.
/// Eagerly includes sub-entity collections for rich aggregate hydration.
/// </summary>
public sealed class EfCoreIdentityUserRepository
    : EfCoreRepository<IdentityDbContext, IdentityUser, Guid>, IIdentityUserRepository
{
    private static readonly string[] SearchColumns = ["UserName", "Email", "Name", "Surname"];

    public EfCoreIdentityUserRepository(IDbContextProvider<IdentityDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    /// <inheritdoc />
    public override async Task<IdentityUser?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await IncludeAll(dbSet)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IdentityUser?> FindByNormalizedUserNameAsync(
        string normalizedUserName, CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();
        return await IncludeAll(dbSet)
            .FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IdentityUser?> FindByNormalizedEmailAsync(
        string normalizedEmail, CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();
        return await IncludeAll(dbSet)
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IdentityUser?> FindByLoginAsync(
        string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();
        return await IncludeAll(dbSet)
            .Where(u => u.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IdentityUser?> FindByTokenAsync(
        string loginProvider, string name, string value, CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();
        return await IncludeAll(dbSet)
            .Where(u => u.Tokens.Any(t =>
                t.LoginProvider == loginProvider &&
                t.Name == name &&
                t.Value == value &&
                (t.ExpiresAt == null || t.ExpiresAt > DateTimeOffset.UtcNow)))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<IdentityUser>> GetListAsync(
        PagingParam paging,
        SorterParam? sorter = null,
        string? filter = null,
        CMasterStatus? status = null,
        Guid? roleId = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        return await IncludeAll(dbSet)
            .SearchDynamic(filter, SearchColumns)
            .WhereIf(status.HasValue, u => u.Status == status!.Value)
            .WhereIf(roleId.HasValue, u => u.Roles.Any(r => r.RoleId == roleId!.Value))
            .SortByDynamic(sorter, defaultProperty: "UserName")
            .PageBy(paging)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<long> GetCountAsync(
        string? filter, CMasterStatus? status, Guid? roleId,
        CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet.AsQueryable()
            .SearchDynamic(filter, SearchColumns)
            .WhereIf(status.HasValue, u => u.Status == status!.Value)
            .WhereIf(roleId.HasValue, u => u.Roles.Any(r => r.RoleId == roleId!.Value))
            .LongCountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Guid>> GetRoleIdsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Set<IdentityUserRole>()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<IdentityUser>> GetUsersInRoleAsync(
        Guid roleId, CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();
        return await IncludeAll(dbSet)
            .Where(u => u.Roles.Any(r => r.RoleId == roleId))
            .ToListAsync(cancellationToken);
    }

    // ─── Helpers ─────────────────────────────────────────────────────

    private static IQueryable<IdentityUser> IncludeAll(IQueryable<IdentityUser> query)
        => query
            .AsSplitQuery()
            .Include(u => u.Roles)
            .Include(u => u.Claims)
            .Include(u => u.Logins)
            .Include(u => u.Tokens);
}

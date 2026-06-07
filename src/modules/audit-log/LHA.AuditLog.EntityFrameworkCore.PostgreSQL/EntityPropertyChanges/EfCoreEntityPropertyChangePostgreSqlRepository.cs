using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.EntityChangeProperties;
using LHA.Ddd.Application;
using LHA.EntityFrameworkCore;
using LHA.Shared.Domain.EntityPropertyChanges;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace LHA.AuditLog.EntityFrameworkCore.PostgreSQL;

/// <summary>
/// PostgreSQL implementation of <see cref="IEntityPropertyChangeRepository"/>.
/// </summary>
public class EfCoreEntityPropertyChangePostgreSqlRepository
    : EfCoreRepository<AuditLogDbContext, EntityPropertyChangeEntity, Guid>, IEntityPropertyChangeRepository
{

    private readonly IUnitOfWorkManager _unitOfWorkManager;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="unitOfWorkManager">Unit of work manager.</param>
    /// <param name="dbContextProvider">DB context provider.</param>
    public EfCoreEntityPropertyChangePostgreSqlRepository(IDbContextProvider<AuditLogDbContext> dbContextProvider,
        IUnitOfWorkManager unitOfWorkManager)
        : base(dbContextProvider)
    {
        _unitOfWorkManager = unitOfWorkManager;
    }

    public virtual async Task<PagedResultDto<EntityPropertyChangeEntity>> GetWithPaginationAsync(
        EntityPropertyChangePagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        var query1 = dbContext.Set<EntityPropertyChangeEntity>().AsNoTracking();
        query1 = ApplyFilter(query1, request);

        var query2 = dbContext.Set<EntityPropertyChangeEntity>().AsNoTracking();
        query2 = ApplyFilter(query2, request);

        var totalCount = await query2.LongCountAsync(cancellationToken);

        var list = await query1
            .SortByDynamic(request.Sorter, nameof(EntityPropertyChangeEntity.Id), defaultAscending: false)
            .PageBy(request)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<EntityPropertyChangeEntity>(
            totalCount,
            list,
            request.PageNumber,
            request.PageSize);
    }

    public virtual async Task<List<EntityPropertyChangeEntity>> GetListAsync(
        EntityPropertyChangePagedRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EntityPropertyChangeEntity> query = await GetDbSetAsync();

        query = ApplyFilter(query, request);

        return await query
            .SortByDynamic(request.Sorter, nameof(EntityPropertyChangeEntity.Id), defaultAscending: true)
            .PageBy(request)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        EntityPropertyChangePagedRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EntityPropertyChangeEntity> query = await GetDbSetAsync();

        query = ApplyFilter(query, request);

        return await query.LongCountAsync(cancellationToken);
    }

    protected virtual IQueryable<EntityPropertyChangeEntity> ApplyFilter(
        IQueryable<EntityPropertyChangeEntity> query,
        EntityPropertyChangePagedRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            string lowerSearchQuery = request.SearchQuery.ToLowerInvariant();
            string[] allowSearchColumns = request.AllowSearchColumns.Any() ? request.AllowSearchColumns
                : new string[] { nameof(EntityPropertyChangeEntity.PropertyName), nameof(EntityPropertyChangeEntity.PropertyTypeFullName) };

            query = query.SearchDynamic(lowerSearchQuery, allowSearchColumns, Core.CSearchOperatorType.Contains, true, Core.CSearchCombineModeType.Or);
        }

        return query;
    }
}

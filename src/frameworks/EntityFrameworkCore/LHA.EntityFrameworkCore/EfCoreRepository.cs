using LHA.Ddd.Domain;
using Microsoft.EntityFrameworkCore;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// EF Core–based implementation of <see cref="IRepository{TEntity,TKey}"/>.
/// Provides CRUD operations backed by a <see cref="DbContext"/> resolved via
/// <see cref="IDbContextProvider{TDbContext}"/>.
/// </summary>
/// <typeparam name="TDbContext">The DbContext type.</typeparam>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity key type.</typeparam>
public class EfCoreRepository<TDbContext, TEntity, TKey>
    : IEfCoreRepository<TEntity, TKey>
    where TDbContext : DbContext
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    private readonly IDbContextProvider<TDbContext> _dbContextProvider;

    public EfCoreRepository(IDbContextProvider<TDbContext> dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }

    /// <summary>
    /// Resolves the <see cref="DbContext"/> from the provider.
    /// </summary>
    public async Task<DbContext> GetDbContextAsync() =>
        await _dbContextProvider.GetDbContextAsync();

    /// <summary>
    /// Gets the <see cref="DbSet{TEntity}"/> from the resolved DbContext.
    /// </summary>
    public async Task<DbSet<TEntity>> GetDbSetAsync()
    {
        var dbContext = await _dbContextProvider.GetDbContextAsync();
        return dbContext.Set<TEntity>();
    }

    /// <inheritdoc />
    public virtual async Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FindAsync([id], cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, cancellationToken);
        if (entity is null)
        {
            throw new EntityNotFoundException(typeof(TEntity), id);
        }

        return entity;
    }

    /// <inheritdoc />
    public virtual async Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.LongCountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var entry = await dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    /// <inheritdoc />
    public virtual async Task InsertManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        await dbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var entry = dbContext.Entry(entity);

        // If the entity is not tracked, attach and mark entire graph as Modified.
        // If already tracked (loaded within the same UoW), just ensure it is Modified.
        // Do NOT call dbContext.Update() on tracked entities — it cascades through the
        // entire navigation graph, turning Added children into Modified.
        if (entry.State == EntityState.Detached)
        {
            dbContext.Update(entity);
        }
        else if (entry.State == EntityState.Unchanged)
        {
            entry.State = EntityState.Modified;
        }

        return entity;
    }

    /// <inheritdoc />
    public virtual async Task UpdateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        foreach (var entity in entities)
        {
            var entry = dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                dbContext.Update(entity);
            }
            else if (entry.State == EntityState.Unchanged)
            {
                entry.State = EntityState.Modified;
            }
        }
    }

    /// <inheritdoc />
    public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        dbContext.Set<TEntity>().Remove(entity);
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(id, cancellationToken);
        await DeleteAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task DeleteManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        dbContext.Set<TEntity>().RemoveRange(entities);
        await Task.CompletedTask;
    }
}

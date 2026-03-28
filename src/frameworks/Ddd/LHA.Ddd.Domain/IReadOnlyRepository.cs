namespace LHA.Ddd.Domain;

/// <summary>
/// Read-only repository abstraction for querying entities.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity key type.</typeparam>
public interface IReadOnlyRepository<TEntity, in TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    /// <summary>
    /// Finds an entity by its identifier, returning <see langword="null"/> if not found.
    /// </summary>
    Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity by its identifier, throwing <see cref="EntityNotFoundException"/> if not found.
    /// </summary>
    Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all entities of this type.
    /// </summary>
    Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the total count of entities.
    /// </summary>
    Task<long> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a paged list of entities.
    /// </summary>
    Task<List<TEntity>> GetListAsync(
        PagingParam paging,
        string? sorting = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a paged, sorted, and filtered list of entities.
    /// </summary>
    Task<List<TEntity>> GetListAsync<TFilter>(
        TableParam<TFilter> input,
        CancellationToken cancellationToken = default);
}

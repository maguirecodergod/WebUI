using LHA.Ddd.Domain;

namespace LHA.Ddd.Application;

/// <summary>
/// Base class for read-only application services that support getting a single entity
/// and a paged list. Subclasses must implement entity-to-DTO mapping.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TEntityDto">The DTO type.</typeparam>
/// <typeparam name="TKey">The entity key type.</typeparam>
/// <typeparam name="TGetListInput">The input type for list queries.</typeparam>
public abstract class ReadOnlyAppService<TEntity, TEntityDto, TKey, TGetListInput>
    : ApplicationService, IReadOnlyAppService<TEntityDto, TKey, TGetListInput>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    /// <summary>
    /// The repository used for reading entities.
    /// </summary>
    protected IReadOnlyRepository<TEntity, TKey> ReadOnlyRepository { get; }

    protected ReadOnlyAppService(IReadOnlyRepository<TEntity, TKey> repository)
    {
        ReadOnlyRepository = repository;
    }

    /// <inheritdoc />
    public virtual async Task<TEntityDto> GetAsync(TKey id)
    {
        var entity = await ReadOnlyRepository.GetAsync(id);
        return await EnrichAuditAsync(MapToDto(entity));
    }

    /// <inheritdoc />
    public abstract Task<PagedResultDto<TEntityDto>> GetListAsync(TGetListInput input);

    /// <summary>
    /// Maps an entity to its DTO representation.
    /// </summary>
    protected abstract TEntityDto MapToDto(TEntity entity);
}

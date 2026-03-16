using LHA.Ddd.Domain;

namespace LHA.Ddd.Application;

/// <summary>
/// Base class for CRUD application services. Subclasses must implement entity-to-DTO mapping
/// and input-to-entity mapping for create and update operations.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TEntityDto">The DTO type.</typeparam>
/// <typeparam name="TKey">The entity key type.</typeparam>
/// <typeparam name="TGetListInput">The input type for list queries.</typeparam>
/// <typeparam name="TCreateInput">The input type for creating an entity.</typeparam>
/// <typeparam name="TUpdateInput">The input type for updating an entity.</typeparam>
public abstract class CrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
    : ReadOnlyAppService<TEntity, TEntityDto, TKey, TGetListInput>,
      ICrudAppService<TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    /// <summary>
    /// The repository used for CRUD operations.
    /// </summary>
    protected IRepository<TEntity, TKey> Repository { get; }

    protected CrudAppService(IRepository<TEntity, TKey> repository)
        : base(repository)
    {
        Repository = repository;
    }

    /// <inheritdoc />
    public virtual async Task<TEntityDto> CreateAsync(TCreateInput input)
    {
        var entity = MapToEntity(input);
        entity = await Repository.InsertAsync(entity);
        return MapToDto(entity);
    }

    /// <inheritdoc />
    public virtual async Task<TEntityDto> UpdateAsync(TKey id, TUpdateInput input)
    {
        var entity = await Repository.GetAsync(id);
        MapToEntity(input, entity);
        entity = await Repository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    /// <inheritdoc />
    public virtual async Task DeleteAsync(TKey id)
    {
        await Repository.DeleteAsync(id);
    }

    /// <summary>
    /// Creates a new entity from the create input.
    /// </summary>
    protected abstract TEntity MapToEntity(TCreateInput input);

    /// <summary>
    /// Applies the update input to an existing entity.
    /// </summary>
    protected abstract void MapToEntity(TUpdateInput input, TEntity entity);
}

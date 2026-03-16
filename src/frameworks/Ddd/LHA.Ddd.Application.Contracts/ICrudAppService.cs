namespace LHA.Ddd.Application;

/// <summary>
/// Full CRUD application service interface.
/// </summary>
/// <typeparam name="TEntityDto">The DTO type returned to clients.</typeparam>
/// <typeparam name="TKey">The entity key type.</typeparam>
/// <typeparam name="TGetListInput">The input type for list queries.</typeparam>
/// <typeparam name="TCreateInput">The input type for creating an entity.</typeparam>
/// <typeparam name="TUpdateInput">The input type for updating an entity.</typeparam>
public interface ICrudAppService<TEntityDto, in TKey, in TGetListInput, in TCreateInput, in TUpdateInput>
    : IReadOnlyAppService<TEntityDto, TKey, TGetListInput>
{
    /// <summary>
    /// Creates a new entity.
    /// </summary>
    Task<TEntityDto> CreateAsync(TCreateInput input);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    Task<TEntityDto> UpdateAsync(TKey id, TUpdateInput input);

    /// <summary>
    /// Deletes an entity by its identifier.
    /// </summary>
    Task DeleteAsync(TKey id);
}

/// <summary>
/// CRUD application service where the create and update input types are the same as the DTO.
/// </summary>
public interface ICrudAppService<TEntityDto, in TKey, in TGetListInput>
    : ICrudAppService<TEntityDto, TKey, TGetListInput, TEntityDto, TEntityDto>;

/// <summary>
/// CRUD application service using defaults for both listing and input types.
/// </summary>
public interface ICrudAppService<TEntityDto, in TKey>
    : ICrudAppService<TEntityDto, TKey, PagedAndSortedResultRequestDto>;

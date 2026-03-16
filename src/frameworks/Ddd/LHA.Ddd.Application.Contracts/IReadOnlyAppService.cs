namespace LHA.Ddd.Application;

/// <summary>
/// Read-only application service that supports getting a single entity and a paged list.
/// </summary>
/// <typeparam name="TEntityDto">The DTO type returned to clients.</typeparam>
/// <typeparam name="TKey">The entity key type.</typeparam>
/// <typeparam name="TGetListInput">The input type for list queries.</typeparam>
public interface IReadOnlyAppService<TEntityDto, in TKey, in TGetListInput> : IApplicationService
{
    /// <summary>
    /// Gets a single entity by identifier.
    /// </summary>
    Task<TEntityDto> GetAsync(TKey id);

    /// <summary>
    /// Gets a paged list of entities.
    /// </summary>
    Task<PagedResultDto<TEntityDto>> GetListAsync(TGetListInput input);
}

/// <summary>
/// Read-only application service using the default <see cref="PagedAndSortedResultRequestDto"/> for listing.
/// </summary>
public interface IReadOnlyAppService<TEntityDto, in TKey>
    : IReadOnlyAppService<TEntityDto, TKey, PagedAndSortedResultRequestDto>;

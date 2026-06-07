using LHA.Ddd.Application;

namespace LHA.Shared.Domain.EntityPropertyChanges
{
    /// <summary>
    /// Paged request for querying entity property changes with filtering and sorting.
    /// </summary>
    public class EntityPropertyChangePagedRequest : PagedAndSortedResultRequestDto<EntityPropertyChangeFilter>
    {
    }

    /// <summary>
    /// Paged query for querying entity property changes.
    /// </summary>
    public class EntityPropertyChangePagedQuery : PagedAndSortedResultRequestDto
    {
        public EntityPropertyChangePagedRequest ToRequest()
        {
            return new EntityPropertyChangePagedRequest
            {
                SearchQuery = SearchQuery,
                AllowSearchColumns = AllowSearchColumns,
                SorterKey = SorterKey,
                SorterIsAsc = SorterIsAsc,
                PageNumber = PageNumber,
                PageSize = PageSize
            };
        }
    }

    /// <summary>
    /// Filter for entity property change queries.
    /// </summary>
    public class EntityPropertyChangeFilter
    {

    }
}

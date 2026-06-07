using LHA.Auditing;
using LHA.Ddd.Application;

namespace LHA.Shared.Domain.EntityChanges
{
    /// <summary>
    /// Input for querying entity changes with filtering and paging.
    /// </summary>
    public class EntityChangePagedRequest : PagedAndSortedResultRequestDto<EntityChangeFilter>
    {
        /// <summary>
        /// Filters entity changes belonging to a specific parent audit log entry.
        /// </summary>
        public Guid? AuditLogId { get; set; }

        /// <summary>
        /// Filters by the fully-qualified CLR type name of the changed entity.
        /// </summary>
        public string? EntityTypeFullName { get; set; }

        /// <summary>
        /// Filters by the primary key of the changed entity (stringified).
        /// </summary>
        public string? EntityId { get; set; }
    }

    /// <summary>
    /// Paged query for querying entity changes.
    /// </summary>
    public class EntityChangePagedQuery : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Filters entity changes belonging to a specific parent audit log entry.
        /// </summary>
        public Guid? AuditLogId { get; set; }
        /// <summary>
        /// Filters by the fully-qualified CLR type name of the changed entity.
        /// </summary>
        public string? EntityTypeFullName { get; set; }
        /// <summary>
        /// Filters by the primary key of the changed entity (stringified).
        /// </summary>
        public string? EntityId { get; set; }
        /// <summary>
        /// Filters by one or more change types (<see cref="CEntityChangeType"/>).
        /// </summary>
        public CEntityChangeType[] ChangeTypes { get; set; } = [];

        public EntityChangePagedRequest ToRequest()
        {
            return new EntityChangePagedRequest
            {
                AuditLogId = AuditLogId,
                EntityTypeFullName = EntityTypeFullName,
                EntityId = EntityId,
                SearchQuery = SearchQuery,
                AllowSearchColumns = AllowSearchColumns,
                SorterKey = SorterKey,
                SorterIsAsc = SorterIsAsc,
                PageNumber = PageNumber,
                PageSize = PageSize,
                Filter = ChangeTypes.Length > 0
                    ? new EntityChangeFilter { ChangeTypes = ChangeTypes.ToList() }
                    : null
            };
        }
    }

    /// <summary>
    /// Filter for entity change queries.
    /// </summary>
    public class EntityChangeFilter
    {
        /// <summary>
        /// Filters by one or more change types (<see cref="CEntityChangeType"/>).
        /// </summary>
        public List<CEntityChangeType>? ChangeTypes { get; set; }
    }
}

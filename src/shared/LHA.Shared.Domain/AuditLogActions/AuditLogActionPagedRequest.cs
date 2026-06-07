using LHA.Ddd.Application;

namespace LHA.Shared.Domain.AuditLogActions
{
    /// <summary>
    /// Paged request for querying audit log actions with filtering and sorting.
    /// </summary>
    public class AuditLogActionPagedRequest : PagedAndSortedResultRequestDto<AuditLogActionFilter>
    {
        /// <summary>
        /// Filters actions belonging to a specific parent audit log entry.
        /// </summary>
        public Guid? AuditLogId { get; set; }
        // /// <summary>
        // /// Service name
        // /// </summary>
        // public string? ServiceName { get; set; }
        // /// <summary>
        // /// Method name
        // /// </summary>
        // public string? MethodName { get; set; }
    }

    /// <summary>
    /// Paged query for querying audit log actions.
    /// </summary>
    public class AuditLogActionPagedQuery : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Filters actions belonging to a specific parent audit log entry.
        /// </summary>
        public Guid? AuditLogId { get; set; }
        /// <summary>
        /// Lower bound for the execution time filter (inclusive).
        /// </summary>
        public DateTimeOffset? ExecutionStartTime { get; set; }
        /// <summary>
        /// Upper bound for the execution time filter (inclusive).
        /// </summary>
        public DateTimeOffset? ExecutionEndTime { get; set; }
        /// <summary>
        /// Minimum execution duration filter in milliseconds (inclusive).
        /// </summary>
        public int? MinExecutionDuration { get; set; }
        /// <summary>
        /// Maximum execution duration filter in milliseconds (inclusive).
        /// </summary>
        public int? MaxExecutionDuration { get; set; }

        /// <summary>
        /// Converts the query to a request.
        /// </summary>
        /// <returns></returns>
        public AuditLogActionPagedRequest ToRequest()
        {
            return new AuditLogActionPagedRequest
            {
                AuditLogId = AuditLogId,
                SearchQuery = SearchQuery,
                AllowSearchColumns = AllowSearchColumns,
                SorterKey = SorterKey,
                SorterIsAsc = SorterIsAsc,
                PageNumber = PageNumber,
                PageSize = PageSize,
                Filter = HasFilter()
                    ? new AuditLogActionFilter
                    {
                        ExecutionStartTime = ExecutionStartTime,
                        ExecutionEndTime = ExecutionEndTime,
                        MinExecutionDuration = MinExecutionDuration,
                        MaxExecutionDuration = MaxExecutionDuration
                    }
                    : null
            };
        }

        private bool HasFilter()
        {
            return ExecutionStartTime.HasValue
                   || ExecutionEndTime.HasValue
                   || MinExecutionDuration.HasValue
                   || MaxExecutionDuration.HasValue;
        }
    }

    /// <summary>
    /// Filter for audit log action queries.
    /// </summary>
    public class AuditLogActionFilter
    {
        /// <summary>
        /// Lower bound for the execution time filter (inclusive).
        /// </summary>
        public DateTimeOffset? ExecutionStartTime { get; set; } = null;
        /// <summary>
        /// Upper bound for the execution time filter (inclusive).
        /// </summary>
        public DateTimeOffset? ExecutionEndTime { get; set; } = null;

        /// <summary>
        /// Minimum execution duration filter in milliseconds (inclusive).
        /// </summary>
        public int? MinExecutionDuration { get; set; } = null;
        /// <summary>
        /// Maximum execution duration filter in milliseconds (inclusive).
        /// </summary>
        public int? MaxExecutionDuration { get; set; } = null;
    }
}

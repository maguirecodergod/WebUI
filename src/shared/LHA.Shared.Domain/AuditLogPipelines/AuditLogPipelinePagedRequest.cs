using LHA.Ddd.Application;

namespace LHA.Shared.Domain.AuditLogPipelines
{
    /// <summary>
    /// Paged request for Audit Log Pipeline entities.
    /// </summary>
    public class AuditLogPipelinePagedRequest : PagedAndSortedResultRequestDto<AuditLogPipelineFilter>
    {
    }

    /// <summary>
    /// Filter criteria for Audit Log Pipeline entities.
    /// </summary>
    public class AuditLogPipelineFilter
    {
        /// <summary>
        /// Timestamp start filter
        /// </summary>
        public DateTimeOffset? TimestampStart { get; set; }

        /// <summary>
        /// Timestamp end filter
        /// </summary>
        public DateTimeOffset? TimestampEnd { get; set; }

        /// <summary>
        /// Service name filter
        /// </summary>
        public string? ServiceName { get; set; }

        /// <summary>
        /// Status filter
        /// </summary>
        public List<byte>? Statuses { get; set; } = null;

        /// <summary>
        /// Minimum HTTP Status Code filter
        /// </summary>
        public int? MinStatusCode { get; set; } = null;

        /// <summary>
        /// Maximum HTTP Status Code filter
        /// </summary>
        public int? MaxStatusCode { get; set; } = null;

        /// <summary>
        /// HTTP Method filter
        /// </summary>
        public List<string?>? HttpMethods { get; set; } = null;

        /// <summary>
        /// Minimum duration (ms) filter
        /// </summary>
        public long? DurationMin { get; set; }

        /// <summary>
        /// Maximum duration (ms) filter
        /// </summary>
        public long? DurationMax { get; set; }
    }
}
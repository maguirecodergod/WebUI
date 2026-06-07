using LHA.Auditing;
using LHA.Ddd.Application;

namespace LHA.Shared.Domain.AuditLogs;

/// <summary>
/// Paged request for querying audit logs.
/// </summary>
public class AuditLogPagedRequest : PagedAndSortedResultRequestDto<AuditLogFilter>
{
    /// <summary>
    /// When <c>true</c>, includes nested <c>AuditLogActionDto</c> and <c>EntityChangeDto</c> collections in each result.
    /// </summary>
    public bool IncludeDetails { get; set; } = false;

    /// <summary>
    /// When <c>true</c>, bypasses the multi-tenant query filter to return audit logs across ALL tenants. Intended for host-level (super-admin) access only.
    /// </summary>
    public bool DisableTenantFilter { get; set; } = false;
}

/// <summary>
/// Paged query for querying audit logs.
/// </summary>
public class AuditLogPagedQuery : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// When <c>true</c>, includes nested <c>AuditLogActionDto</c> and <c>EntityChangeDto</c> collections in each result.
    /// </summary>
    public bool IncludeDetails { get; set; } = false;
    /// <summary>
    /// When <c>true</c>, bypasses the multi-tenant query filter to return audit logs across ALL tenants. Intended for host-level (super-admin) access only.
    /// </summary>
    public bool DisableTenantFilter { get; set; } = false;
    /// <summary>
    /// Lower bound for the execution time filter (inclusive).
    /// </summary>
    public DateTimeOffset? StartTime { get; set; }
    /// <summary>
    /// Upper bound for the execution time filter (inclusive).
    /// </summary>
    public DateTimeOffset? EndTime { get; set; }
    /// <summary>
    /// Filters by HTTP method (e.g., <c>GET</c>, <c>POST</c>). Case-insensitive.
    /// </summary>
    public string? HttpMethod { get; set; }
    /// <summary>
    /// Filters by request URL. Supports contains matching.
    /// </summary>
    public string? Url { get; set; }
    /// <summary>
    /// Filters by the unique identifier of the user who performed the operation.
    /// </summary>
    public Guid? UserId { get; set; }
    /// <summary>
    /// Filters by user login name. Supports contains matching.
    /// </summary>
    public string? UserName { get; set; }
    /// <summary>
    /// Minimum HTTP status code filter (inclusive).
    /// </summary>
    public int? MinStatusCode { get; set; }
    /// <summary>
    /// Maximum HTTP status code filter (inclusive).
    /// </summary>
    public int? MaxStatusCode { get; set; }
    /// <summary>
    /// Filters by the originating application or microservice name.
    /// </summary>
    public string? ApplicationName { get; set; }
    /// <summary>
    /// Filters by distributed tracing correlation ID (exact match).
    /// </summary>
    public string? CorrelationId { get; set; }
    /// <summary>
    /// Minimum execution duration filter in milliseconds (inclusive).
    /// </summary>
    public int? MinExecutionDuration { get; set; }
    /// <summary>
    /// Maximum execution duration filter in milliseconds (inclusive).
    /// </summary>
    public int? MaxExecutionDuration { get; set; }
    /// <summary>
    /// When <c>true</c>, returns only audit entries that resulted in an exception.
    /// </summary>
    public bool? HasException { get; set; }
    /// <summary>
    /// Filters by the transport channel classification (<see cref="CRequestType"/>).
    /// </summary>
    public CRequestType? RequestType { get; set; }

    /// <summary>
    /// Converts the query to a request.
    /// </summary>
    /// <returns></returns>
    public AuditLogPagedRequest ToRequest()
    {
        return new AuditLogPagedRequest
        {
            IncludeDetails = IncludeDetails,
            DisableTenantFilter = DisableTenantFilter,
            SearchQuery = SearchQuery,
            AllowSearchColumns = AllowSearchColumns,
            SorterKey = SorterKey,
            SorterIsAsc = SorterIsAsc,
            PageNumber = PageNumber,
            PageSize = PageSize,
            Filter = HasFilter()
                ? new AuditLogFilter
                {
                    StartTime = StartTime,
                    EndTime = EndTime,
                    HttpMethod = HttpMethod,
                    Url = Url,
                    UserId = UserId,
                    UserName = UserName,
                    MinStatusCode = MinStatusCode,
                    MaxStatusCode = MaxStatusCode,
                    ApplicationName = ApplicationName,
                    CorrelationId = CorrelationId,
                    MinExecutionDuration = MinExecutionDuration,
                    MaxExecutionDuration = MaxExecutionDuration,
                    HasException = HasException,
                    RequestType = RequestType
                }
                : null
        };
    }

    private bool HasFilter()
    {
        return StartTime.HasValue
               || EndTime.HasValue
               || !string.IsNullOrWhiteSpace(HttpMethod)
               || !string.IsNullOrWhiteSpace(Url)
               || UserId.HasValue
               || !string.IsNullOrWhiteSpace(UserName)
               || MinStatusCode.HasValue
               || MaxStatusCode.HasValue
               || !string.IsNullOrWhiteSpace(ApplicationName)
               || !string.IsNullOrWhiteSpace(CorrelationId)
               || MinExecutionDuration.HasValue
               || MaxExecutionDuration.HasValue
               || HasException.HasValue
               || RequestType.HasValue;
    }
}

/// <summary>
/// Filter for Audit Log queries.
/// </summary>
public class AuditLogFilter
{
    /// <summary>
    /// Lower bound for the execution time filter (inclusive).
    /// </summary>
    public DateTimeOffset? StartTime { get; set; }
    /// <summary>
    /// Upper bound for the execution time filter (inclusive).
    /// </summary>
    public DateTimeOffset? EndTime { get; set; }
    /// <summary>
    /// Filters by HTTP method (e.g., <c>GET</c>, <c>POST</c>). Case-insensitive.
    /// </summary>
    public string? HttpMethod { get; set; }
    /// <summary>
    /// Filters by request URL. Supports contains matching.
    /// </summary>
    public string? Url { get; set; }
    /// <summary>
    /// Filters by the unique identifier of the user who performed the operation.
    /// </summary>
    public Guid? UserId { get; set; }
    /// <summary>
    /// Filters by user login name. Supports contains matching.
    /// </summary>
    public string? UserName { get; set; }
    /// <summary>
    /// Minimum HTTP status code filter (inclusive).
    /// </summary>
    public int? MinStatusCode { get; set; }
    /// <summary>
    /// Maximum HTTP status code filter (inclusive).
    /// </summary>
    public int? MaxStatusCode { get; set; }
    /// <summary>
    /// Filters by the originating application or microservice name.
    /// </summary>
    public string? ApplicationName { get; set; }
    /// <summary>
    /// Filters by distributed tracing correlation ID (exact match).
    /// </summary>
    public string? CorrelationId { get; set; }
    /// <summary>
    /// Minimum execution duration filter in milliseconds (inclusive).
    /// </summary>
    public int? MinExecutionDuration { get; set; }
    /// <summary>
    /// Maximum execution duration filter in milliseconds (inclusive).
    /// </summary>
    public int? MaxExecutionDuration { get; set; }
    /// <summary>
    /// When <c>true</c>, returns only audit entries that resulted in an exception.
    /// </summary>
    public bool? HasException { get; set; }
    /// <summary>
    /// Filters by the transport channel classification (<see cref="CRequestType"/>).
    /// </summary>
    public CRequestType? RequestType { get; set; }
}



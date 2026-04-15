using LHA.Ddd.Application;

namespace LHA.Shared.Domain.Requests;

public class AuditLogGetListInput : PagedAndSortedResultRequestDto<AuditLogFilter>
{
    public bool IncludeDetails { get; set; } = false;
}

public class AuditLogFilter
{
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public string? HttpMethod { get; set; }
    public string? Url { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public int? MinStatusCode { get; set; }
    public int? MaxStatusCode { get; set; }
    public string? ApplicationName { get; set; }
    public string? CorrelationId { get; set; }
    public int? MinExecutionDuration { get; set; }
    public int? MaxExecutionDuration { get; set; }
    public bool? HasException { get; set; }
}

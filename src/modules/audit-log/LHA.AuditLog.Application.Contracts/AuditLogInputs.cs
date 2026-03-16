using LHA.Ddd.Application;

namespace LHA.AuditLog.Application.Contracts;

/// <summary>
/// Input for querying audit logs with filtering and paging.
/// </summary>
public sealed class GetAuditLogsInput : PagedAndSortedResultRequestDto
{
    /// <summary>Start time filter (inclusive).</summary>
    public DateTimeOffset? StartTime { get; init; }

    /// <summary>End time filter (inclusive).</summary>
    public DateTimeOffset? EndTime { get; init; }

    /// <summary>Filter by HTTP method (GET, POST, etc.).</summary>
    public string? HttpMethod { get; init; }

    /// <summary>Filter by URL (contains, case-insensitive).</summary>
    public string? Url { get; init; }

    /// <summary>Minimum HTTP status code.</summary>
    public int? MinStatusCode { get; init; }

    /// <summary>Maximum HTTP status code.</summary>
    public int? MaxStatusCode { get; init; }

    /// <summary>Filter by user identifier.</summary>
    public Guid? UserId { get; init; }

    /// <summary>Filter by user name (contains, case-insensitive).</summary>
    public string? UserName { get; init; }

    /// <summary>Filter by application name.</summary>
    public string? ApplicationName { get; init; }

    /// <summary>Filter by distributed correlation identifier.</summary>
    public string? CorrelationId { get; init; }

    /// <summary>Filter by maximum execution duration in milliseconds.</summary>
    public int? MaxExecutionDuration { get; init; }

    /// <summary>Filter by minimum execution duration in milliseconds.</summary>
    public int? MinExecutionDuration { get; init; }

    /// <summary>Filter by whether exceptions occurred. <c>null</c> returns all.</summary>
    public bool? HasException { get; init; }
}

/// <summary>
/// Input for querying entity changes with filtering and paging.
/// </summary>
public sealed class GetEntityChangesInput : PagedAndSortedResultRequestDto
{
    /// <summary>Filter by entity CLR type full name.</summary>
    public string? EntityTypeFullName { get; init; }

    /// <summary>Filter by entity primary key.</summary>
    public string? EntityId { get; init; }
}

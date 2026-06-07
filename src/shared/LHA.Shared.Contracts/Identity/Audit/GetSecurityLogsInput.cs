using LHA.Ddd.Application;

namespace LHA.Shared.Contracts.Identity.Audit;

/// <summary>Input for querying security logs.</summary>
public sealed class GetSecurityLogsInput : PagedAndSortedResultRequestDto
{
    /// <summary>Free-text filter applied to user name, application name, or action.</summary>
    public string? Filter { get; init; }

    /// <summary>Filters results to security events for the specified user.</summary>
    public Guid? UserId { get; init; }

    /// <summary>Filters results to security events with the specified action code (e.g. <c>"Login"</c>, <c>"ChangePassword"</c>).</summary>
    public string? Action { get; init; }
}

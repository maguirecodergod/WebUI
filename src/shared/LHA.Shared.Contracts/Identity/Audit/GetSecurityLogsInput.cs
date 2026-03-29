using LHA.Ddd.Application;

namespace LHA.Shared.Contracts.Identity.Audit;

/// <summary>Input for querying security logs.</summary>
public sealed class GetSecurityLogsInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; init; }
    public Guid? UserId { get; init; }
    public string? Action { get; init; }
}

using LHA.Ddd.Application;

namespace LHA.Shared.Contracts.Identity.Claims;

/// <summary>Input for querying claim types.</summary>
public sealed class GetClaimTypesInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; init; }
}

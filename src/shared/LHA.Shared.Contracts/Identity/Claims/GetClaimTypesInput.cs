using LHA.Ddd.Application;

namespace LHA.Shared.Contracts.Identity.Claims;

/// <summary>Input for querying claim types.</summary>
public sealed class GetClaimTypesInput : PagedAndSortedResultRequestDto
{
    /// <summary>Free-text filter applied to the claim type name or description.</summary>
    public string? Filter { get; init; }
}

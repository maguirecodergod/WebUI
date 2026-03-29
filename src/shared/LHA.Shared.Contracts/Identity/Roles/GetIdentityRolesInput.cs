using LHA.Core;
using LHA.Ddd.Application;

namespace LHA.Shared.Contracts.Identity.Roles;

/// <summary>Input for querying roles.</summary>
public sealed class GetIdentityRolesInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; init; }
    public CMasterStatus? Status { get; init; }
}

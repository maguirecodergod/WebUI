using LHA.Core;
using LHA.Ddd.Application;

namespace LHA.Shared.Contracts.Identity.Roles;

/// <summary>Input for querying roles.</summary>
public sealed class GetIdentityRolesInput : PagedAndSortedResultRequestDto
{
    /// <summary>Free-text filter applied to role name.</summary>
    public string? Filter { get; set; }

    /// <summary>Filters results to roles with the specified lifecycle status. See <see cref="CMasterStatus"/>.</summary>
    public CMasterStatus? Status { get; set; }
}

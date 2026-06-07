using LHA.Core;
using LHA.Ddd.Application;
using LHA.Shared.Contracts.Identity;

namespace LHA.Shared.Contracts.Identity.Users;

/// <summary>Input for querying users with filtering and paging.</summary>
public sealed class GetIdentityUsersInput : PagedAndSortedResultRequestDto
{
    /// <summary>Free-text filter applied to user name, email, or phone number.</summary>
    public string? Filter { get; set; }

    /// <summary>Filters results to users with the specified lifecycle status. See <see cref="CMasterStatus"/>.</summary>
    public CMasterStatus? Status { get; set; }

    /// <summary>Filters results to users assigned to the specified role.</summary>
    public Guid? RoleId { get; set; }
}

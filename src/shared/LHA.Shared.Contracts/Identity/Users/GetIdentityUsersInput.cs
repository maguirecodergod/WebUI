using LHA.Core;
using LHA.Ddd.Application;
using LHA.Shared.Contracts.Identity;

namespace LHA.Shared.Contracts.Identity.Users;

/// <summary>Input for querying users with filtering and paging.</summary>
public sealed class GetIdentityUsersInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; init; }
    public CMasterStatus? Status { get; init; }
    public Guid? RoleId { get; init; }
}

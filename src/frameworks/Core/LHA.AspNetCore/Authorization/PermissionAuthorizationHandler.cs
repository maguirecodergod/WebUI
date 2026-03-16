using Microsoft.AspNetCore.Authorization;

namespace LHA.AspNetCore.Authorization;

/// <summary>
/// Evaluates <see cref="PermissionRequirement"/> by checking the <c>permissions</c>
/// claim array in the current user's JWT.
/// </summary>
public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private const string PermissionClaimType = "permissions";

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.HasClaim(PermissionClaimType, requirement.PermissionName))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace LHA.AspNetCore.Authorization;

/// <summary>
/// Dynamically creates authorization policies for permission names.
/// Any policy name that is not a built-in/registered policy is treated
/// as a permission requirement.
/// </summary>
public sealed class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options) { }

    /// <inheritdoc />
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Try built-in / explicitly-registered policies first
        var policy = await base.GetPolicyAsync(policyName);
        if (policy is not null) return policy;

        // Treat the policy name as a permission name
        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();
    }
}

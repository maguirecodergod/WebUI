using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace LHA.BlazorWasm.Services.Auth;

public class CustomAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Check if a policy with this name already exists
        var policy = await base.GetPolicyAsync(policyName);

        if (policy == null)
        {
            // If the policy hasn't been defined, dynamically create one that requires the permission
            // This assumes that fallback policy strings map directly to LHA module permission names.
            policy = new AuthorizationPolicyBuilder()
                .RequireClaim("permissions", policyName)
                .Build();
        }

        return policy;
    }
}

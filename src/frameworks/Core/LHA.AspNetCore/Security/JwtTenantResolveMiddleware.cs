using LHA.MultiTenancy;
using Microsoft.AspNetCore.Http;
using LHA.Core.Security;

namespace LHA.AspNetCore.Security;

/// <summary>
/// Middleware that resolves the current tenant from the authenticated user's JWT claims.
/// <para>
/// Reads the <see cref="LhaClaimTypes.TenantId"/> claim from the <c>ClaimsPrincipal</c>
/// and sets <see cref="ICurrentTenant"/> for the lifetime of the request.
/// </para>
/// <para>
/// Must be placed <b>after</b> <c>UseAuthentication()</c> so that
/// <c>HttpContext.User</c> is already populated.
/// </para>
/// </summary>
public sealed class JwtTenantResolveMiddleware
{
    private readonly RequestDelegate _next;

    public JwtTenantResolveMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentTenant currentTenant)
    {
        // Only resolve if no tenant is already set (e.g., by a header-based resolver)
        if (!currentTenant.IsAvailable && context.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst(LhaClaimTypes.TenantId)?.Value;
            if (Guid.TryParse(tenantClaim, out var tenantId))
            {
                using (currentTenant.Change(tenantId))
                {
                    await _next(context);
                    return;
                }
            }
        }

        await _next(context);
    }
}

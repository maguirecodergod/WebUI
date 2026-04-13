using LHA.MultiTenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.AspNetCore.Security;

/// <summary>
/// Middleware that resolves the current tenant from HTTP requests using the registered <c>ITenantResolver</c> pipeline.
/// </summary>
public sealed class MultiTenancyMiddleware
{
    private readonly RequestDelegate _next;

    public MultiTenancyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentTenant currentTenant)
    {
        // Only run resolver if tenant is not already available
        if (!currentTenant.IsAvailable)
        {
            var tenantResolver = context.RequestServices.GetRequiredService<ITenantResolver>();
            var resolveResult = await tenantResolver.ResolveAsync(context.RequestAborted);

            if (resolveResult.TenantIdOrName != null)
            {
                if (Guid.TryParse(resolveResult.TenantIdOrName, out var tenantId))
                {
                    using (currentTenant.Change(tenantId))
                    {
                        await _next(context);
                        return;
                    }
                }
                else
                {
                    // Fallback to searching TenantStore by name if string is provided
                    var tenantStore = context.RequestServices.GetRequiredService<ITenantStore>();
                    var tenantInfo = await tenantStore.FindByNameAsync(resolveResult.TenantIdOrName);
                    if (tenantInfo != null)
                    {
                         using (currentTenant.Change(tenantInfo.Id, tenantInfo.Name))
                         {
                              await _next(context);
                              return;
                         }
                    }
                }
            }
        }

        await _next(context);
    }
}

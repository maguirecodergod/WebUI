using LHA.MultiTenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.AspNetCore.Security;

public class HttpHeaderTenantResolveContributor : TenantResolveContributorBase
{
    public const string TenantIdHeaderName = "X-Tenant-ID";
    public const string TenantIdQueryStringName = "__tenant";

    public override string Name => "HttpHeader";

    protected override Task<string?> ResolveIdOrNameAsync(TenantResolveContext context)
    {
        var httpContextAccessor = context.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return Task.FromResult<string?>(null);
        }

        // 1. Try from Header
        if (httpContext.Request.Headers.TryGetValue(TenantIdHeaderName, out var tenantValues) && tenantValues.Count > 0)
        {
            var value = tenantValues.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return Task.FromResult<string?>(value);
            }
        }

        // 2. Try from Query String
        if (httpContext.Request.Query.TryGetValue(TenantIdQueryStringName, out var queryValues) && queryValues.Count > 0)
        {
            var value = queryValues.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return Task.FromResult<string?>(value);
            }
        }

        return Task.FromResult<string?>(null);
    }
}

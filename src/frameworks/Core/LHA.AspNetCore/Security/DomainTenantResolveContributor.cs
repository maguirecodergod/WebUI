using System.Text.RegularExpressions;
using LHA.MultiTenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.AspNetCore.Security;

/// <summary>
/// Resolves tenant from the Host header (e.g., subdomain).
/// Example: tenant1.lhaapp.com -> tenant1
/// </summary>
public class DomainTenantResolveContributor : TenantResolveContributorBase
{
    public override string Name => "Domain";

    // You can customize this via options if needed. 
    // Here we assume {0}.domain.com or just taking the first part of the subdomain.
    private const string DomainFormat = "{0}."; 

    protected override Task<string?> ResolveIdOrNameAsync(TenantResolveContext context)
    {
        var httpContextAccessor = context.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        var httpContext = httpContextAccessor.HttpContext;
        
        if (httpContext is null)
        {
            return Task.FromResult<string?>(null);
        }

        var host = httpContext.Request.Host.Host;

        // Parts split by '.'
        // Case 1: tenant1.lhaapp.com -> ["tenant1", "lhaapp", "com"] (Length 3)
        // Case 2: tenant1.localhost -> ["tenant1", "localhost"] (Length 2)
        var parts = host.Split('.');
        
        if (parts.Length >= 2)
        {
            var potentialTenant = parts[0];

            // Exclude common root domains/parts that shouldn't be treated as tenants
            var excludedKeywords = new[] { "www", "localhost", "127", "api", "shop" };
            
            if (!excludedKeywords.Contains(potentialTenant.ToLowerInvariant()))
            {
                return Task.FromResult<string?>(potentialTenant);
            }
        }

        return Task.FromResult<string?>(null);
    }
}

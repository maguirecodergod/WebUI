using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LHA.MultiTenancy;

/// <summary>
/// Pipeline-based tenant resolver that executes each <see cref="ITenantResolveContributor"/>
/// in order until one resolves the tenant.
/// </summary>
internal sealed class TenantResolver : ITenantResolver
{
    private readonly MultiTenancyOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TenantResolver> _logger;

    public TenantResolver(
        IOptions<MultiTenancyOptions> options,
        IServiceProvider serviceProvider,
        ILogger<TenantResolver>? logger = null)
    {
        _options = options.Value;
        _serviceProvider = serviceProvider;
        _logger = logger ?? NullLogger<TenantResolver>.Instance;
    }

    /// <inheritdoc />
    public async Task<TenantResolveResult> ResolveAsync(CancellationToken cancellationToken = default)
    {
        var result = new TenantResolveResult();

        using var scope = _serviceProvider.CreateScope();
        var context = new TenantResolveContext(scope.ServiceProvider);

        foreach (var contributor in _options.TenantResolvers)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogDebug("Trying tenant resolver '{Resolver}'.", contributor.Name);
            await contributor.ResolveAsync(context);
            result.AppliedResolvers.Add(contributor.Name);

            if (context.HasResolved)
            {
                result.TenantIdOrName = context.TenantIdOrName;
                _logger.LogDebug("Tenant resolved by '{Resolver}' as '{TenantIdOrName}'.",
                    contributor.Name, result.TenantIdOrName ?? "(host)");
                return result;
            }
        }

        // Fallback
        if (result.TenantIdOrName is null && !string.IsNullOrWhiteSpace(_options.FallbackTenant))
        {
            result.TenantIdOrName = _options.FallbackTenant;
            result.AppliedResolvers.Add("FallbackTenant");
            _logger.LogDebug("Using fallback tenant '{FallbackTenant}'.", result.TenantIdOrName);
        }

        return result;
    }
}

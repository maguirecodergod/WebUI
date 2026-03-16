namespace LHA.MultiTenancy;

/// <summary>
/// Context passed through the tenant resolution pipeline.
/// </summary>
public sealed class TenantResolveContext
{
    /// <summary>
    /// Service provider for resolving dependencies during resolution.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Set by a contributor to indicate the resolved tenant ID or name.
    /// </summary>
    public string? TenantIdOrName { get; set; }

    /// <summary>
    /// Set to <see langword="true"/> to stop further pipeline processing.
    /// </summary>
    public bool Handled { get; set; }

    public TenantResolveContext(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// Whether a tenant (or explicit host) has been resolved.
    /// </summary>
    public bool HasResolved => Handled || TenantIdOrName is not null;
}

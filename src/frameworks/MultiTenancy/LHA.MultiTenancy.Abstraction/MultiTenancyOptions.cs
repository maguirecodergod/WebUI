namespace LHA.MultiTenancy;

/// <summary>
/// Global multi-tenancy configuration.
/// </summary>
public sealed class MultiTenancyOptions
{
    /// <summary>
    /// Master switch to enable/disable multi-tenancy.
    /// Default: <see langword="false"/>.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Ordered list of tenant resolve contributors for the pipeline.
    /// </summary>
    public List<ITenantResolveContributor> TenantResolvers { get; } = [];

    /// <summary>
    /// Fallback tenant ID or name when no resolver matches.
    /// <see langword="null"/> means fall back to host context.
    /// </summary>
    public string? FallbackTenant { get; set; }

    /// <summary>
    /// When <see langword="true"/>, the framework enforces that events and data
    /// for a tenant are only processed in the tenant's assigned <see cref="DataResidencyRegion"/>.
    /// Default: <see langword="false"/>.
    /// </summary>
    public bool EnforceDataResidency { get; set; }

    /// <summary>
    /// The region this service instance is deployed in.
    /// Used to compare against tenant region for data residency enforcement.
    /// </summary>
    public DataResidencyRegion? CurrentServiceRegion { get; set; }
}

namespace LHA.MultiTenancy;

/// <summary>
/// Result of the tenant resolution pipeline.
/// </summary>
public sealed class TenantResolveResult
{
    /// <summary>
    /// The resolved tenant ID or name, or <see langword="null"/> for the host.
    /// </summary>
    public string? TenantIdOrName { get; set; }

    /// <summary>
    /// Ordered list of contributors that were executed during resolution.
    /// </summary>
    public List<string> AppliedResolvers { get; } = [];
}

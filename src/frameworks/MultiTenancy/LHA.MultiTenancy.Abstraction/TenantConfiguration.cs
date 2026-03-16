using LHA.Core;

namespace LHA.MultiTenancy;

/// <summary>
/// Full configuration for a tenant, including connection strings and regional settings.
/// </summary>
public sealed class TenantConfiguration
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public string NormalizedName { get; set; } = string.Empty;

    /// <summary>
    /// Per-tenant connection strings keyed by logical name.
    /// A <c>"Default"</c> key may serve as the fallback.
    /// </summary>
    public Dictionary<string, string> ConnectionStrings { get; init; } = [];

    /// <summary>
    /// Whether this tenant is active and accepting requests.
    /// Inactive tenants receive a 403/service-unavailable response.
    /// </summary>
    public CMasterStatus Status { get; set; } = CMasterStatus.Active;

    /// <summary>
    /// The data residency region this tenant's data must stay within.
    /// Used for multi-region deployment and compliance.
    /// </summary>
    public DataResidencyRegion Region { get; set; } = DataResidencyRegion.Default;

    /// <summary>
    /// Optional metadata for extensibility.
    /// </summary>
    public Dictionary<string, object> Properties { get; init; } = [];
}

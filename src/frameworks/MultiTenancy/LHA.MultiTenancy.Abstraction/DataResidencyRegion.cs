namespace LHA.MultiTenancy;

/// <summary>
/// Represents the data residency region a tenant is assigned to.
/// Controls where tenant data is stored and processed for regulatory compliance.
/// </summary>
public sealed record DataResidencyRegion
{
    /// <summary>
    /// The region code (e.g., "EU", "US-EAST", "APAC", "AU").
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Human-readable description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Default region for tenants that don't have a specific assignment.
    /// </summary>
    public static readonly DataResidencyRegion Default = new() { Code = "DEFAULT", Description = "Default region (no restriction)" };

    public static readonly DataResidencyRegion EU = new() { Code = "EU", Description = "European Union" };
    public static readonly DataResidencyRegion US = new() { Code = "US", Description = "United States" };
    public static readonly DataResidencyRegion APAC = new() { Code = "APAC", Description = "Asia-Pacific" };

    public override string ToString() => Code;
}

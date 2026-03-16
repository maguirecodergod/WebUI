namespace LHA.BlazorWasm.UI.Tenant;

/// <summary>
/// Represents the current tenant context.
/// </summary>
public interface ITenantContext
{
    /// <summary> Current tenant ID. Null for host-level operations. </summary>
    Guid? TenantId { get; }

    /// <summary> Current tenant name. </summary>
    string? TenantName { get; }

    /// <summary> Whether the user is currently operating as a host (non-tenant). </summary>
    bool IsHost { get; }

    /// <summary> Switch to a different tenant. </summary>
    Task SwitchTenantAsync(Guid? tenantId, string? tenantName);

    /// <summary> Switch to host-level context. </summary>
    Task SwitchToHostAsync();

    /// <summary> Event raised when the active tenant changes. </summary>
    event Action<TenantChangedEventArgs>? OnTenantChanged;
}

/// <summary>
/// Event args for tenant change events.
/// </summary>
public sealed class TenantChangedEventArgs
{
    public Guid? PreviousTenantId { get; init; }
    public Guid? NewTenantId { get; init; }
    public string? NewTenantName { get; init; }
}

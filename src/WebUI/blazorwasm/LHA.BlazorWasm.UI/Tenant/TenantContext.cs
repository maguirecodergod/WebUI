namespace LHA.BlazorWasm.UI.Tenant;

/// <summary>
/// Default implementation of the tenant context.
/// Manages the current tenant state for the SaaS application.
/// </summary>
public sealed class TenantContext : ITenantContext
{
    public Guid? TenantId { get; private set; }
    public string? TenantName { get; private set; }
    public bool IsHost => TenantId is null;

    public event Action<TenantChangedEventArgs>? OnTenantChanged;

    public Task SwitchTenantAsync(Guid? tenantId, string? tenantName)
    {
        var previousId = TenantId;
        TenantId = tenantId;
        TenantName = tenantName;

        OnTenantChanged?.Invoke(new TenantChangedEventArgs
        {
            PreviousTenantId = previousId,
            NewTenantId = tenantId,
            NewTenantName = tenantName
        });

        return Task.CompletedTask;
    }

    public Task SwitchToHostAsync()
    {
        return SwitchTenantAsync(null, null);
    }
}

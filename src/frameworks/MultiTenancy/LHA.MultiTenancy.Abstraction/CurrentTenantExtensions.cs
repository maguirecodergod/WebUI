namespace LHA.MultiTenancy;

/// <summary>
/// Convenience extensions for <see cref="ICurrentTenant"/>.
/// </summary>
public static class CurrentTenantExtensions
{
    /// <summary>
    /// Gets the current tenant ID, throwing if not in a tenant context.
    /// </summary>
    public static Guid GetId(this ICurrentTenant currentTenant)
    {
        ArgumentNullException.ThrowIfNull(currentTenant);
        return currentTenant.Id ?? throw new InvalidOperationException("Not in a tenant context. Current tenant ID is null.");
    }

    /// <summary>
    /// Returns which side (Host or Tenant) the current context represents.
    /// </summary>
    public static CMultiTenancySidesType GetSide(this ICurrentTenant currentTenant)
    {
        ArgumentNullException.ThrowIfNull(currentTenant);
        return currentTenant.Id.HasValue ? CMultiTenancySidesType.Tenant : CMultiTenancySidesType.Host;
    }
}

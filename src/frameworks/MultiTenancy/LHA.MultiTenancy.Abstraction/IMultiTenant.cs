namespace LHA.MultiTenancy;

/// <summary>
/// Marker interface for entities that belong to a specific tenant.
/// </summary>
public interface IMultiTenant
{
    /// <summary>
    /// The tenant this entity belongs to. <see langword="null"/> indicates the host.
    /// </summary>
    Guid? TenantId { get; }
}

namespace LHA.MultiTenancy;

/// <summary>
/// Resolves the current tenant from the ambient request context
/// using a pipeline of <see cref="ITenantResolveContributor"/> instances.
/// </summary>
public interface ITenantResolver
{
    /// <summary>
    /// Resolves the current tenant ID or name.
    /// Returns <see langword="null"/> <see cref="TenantResolveResult.TenantIdOrName"/>
    /// if no tenant could be determined (i.e., host context).
    /// </summary>
    Task<TenantResolveResult> ResolveAsync(CancellationToken cancellationToken = default);
}

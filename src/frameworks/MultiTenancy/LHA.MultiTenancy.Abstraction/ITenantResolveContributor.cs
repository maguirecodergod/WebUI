namespace LHA.MultiTenancy;

/// <summary>
/// A step in the tenant resolution pipeline.
/// Each contributor examines the context and optionally sets the tenant identity.
/// </summary>
public interface ITenantResolveContributor
{
    /// <summary>
    /// Unique name identifying this contributor (for diagnostics).
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Examines the context and sets <see cref="TenantResolveContext.TenantIdOrName"/>
    /// if the tenant can be determined. Set <see cref="TenantResolveContext.Handled"/>
    /// to stop further pipeline processing.
    /// </summary>
    Task ResolveAsync(TenantResolveContext context);
}

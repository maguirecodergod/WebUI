namespace LHA.MultiTenancy;

/// <summary>
/// Abstract base that simplifies creating <see cref="ITenantResolveContributor"/> implementations
/// by providing a typed <see cref="ResolveIdOrNameAsync"/> override.
/// </summary>
public abstract class TenantResolveContributorBase : ITenantResolveContributor
{
    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public async Task ResolveAsync(TenantResolveContext context)
    {
        var idOrName = await ResolveIdOrNameAsync(context);

        if (!string.IsNullOrWhiteSpace(idOrName))
        {
            context.TenantIdOrName = idOrName;
            context.Handled = true;
        }
    }

    /// <summary>
    /// Override to return the tenant identifier or name from the current context.
    /// Return <c>null</c> to indicate the contributor cannot resolve.
    /// </summary>
    protected abstract Task<string?> ResolveIdOrNameAsync(TenantResolveContext context);
}

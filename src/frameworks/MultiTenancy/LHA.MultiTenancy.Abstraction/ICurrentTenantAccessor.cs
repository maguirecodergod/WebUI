namespace LHA.MultiTenancy;

/// <summary>
/// Low-level accessor for the ambient tenant info.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>A <see langword="null"/> <see cref="Current"/> indicates the tenant has not been set explicitly.</item>
/// <item>A non-null <see cref="Current"/> with <see langword="null"/> <c>TenantId</c> means the host scope is active.</item>
/// <item>A non-null <see cref="Current"/> with a non-null <c>TenantId</c> means a tenant scope is active.</item>
/// </list>
/// </remarks>
public interface ICurrentTenantAccessor
{
    BasicTenantInfo? Current { get; set; }
}

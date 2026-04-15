namespace LHA.PermissionManagement.Domain.Shared;

/// <summary>
/// Defines which multi-tenancy side(s) a permission is available for.
/// </summary>
public enum MultiTenancySides
{
    /// <summary>
    /// Permission is available on both Host and Tenant sides.
    /// </summary>
    Both = 0,

    /// <summary>
    /// Permission is only available on the Host side (e.g., tenant management).
    /// </summary>
    Host = 1,

    /// <summary>
    /// Permission is only available on the Tenant side.
    /// </summary>
    Tenant = 2,
}

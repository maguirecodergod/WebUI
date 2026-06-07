namespace LHA.MultiTenancy;

/// <summary>
/// Represents sides in a multi-tenancy application.
/// </summary>
[Flags]
public enum CMultiTenancySidesType : byte
{
    /// <summary>
    /// 1 - Tenant: Tenant side.
    /// </summary>
    Tenant = 1,

    /// <summary>
    /// 2 - Host: Host (platform) side.
    /// </summary>
    Host = 2,

    /// <summary>Both tenant and host.</summary>
    Both = Tenant | Host
}

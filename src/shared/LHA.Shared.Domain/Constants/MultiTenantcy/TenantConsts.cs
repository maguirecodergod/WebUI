namespace LHA.Shared.Domain.TenantManagement;

/// <summary>
/// Module-level constants for the tenant management bounded context.
/// </summary>
public static class TenantConsts
{
    /// <summary>Maximum length of a tenant name.</summary>
    public const int MaxNameLength = 64;

    /// <summary>Maximum length of a connection string logical name (e.g., "Default").</summary>
    public const int MaxConnectionStringNameLength = 128;

    /// <summary>Maximum length of a connection string value.</summary>
    public const int MaxConnectionStringValueLength = 1024;

    /// <summary>Default connection string key.</summary>
    public const string DefaultConnectionStringName = "Default";
}

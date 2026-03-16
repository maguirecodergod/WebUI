namespace LHA.TenantManagement.Application.Contracts;

/// <summary>
/// Permission name constants for the Tenant Management module.
/// </summary>
public static class TenantManagementPermissions
{
    public static class Tenants
    {
        public const string Read = "tenants.read";
        public const string Create = "tenants.create";
        public const string Update = "tenants.update";
        public const string Delete = "tenants.delete";
    }
}

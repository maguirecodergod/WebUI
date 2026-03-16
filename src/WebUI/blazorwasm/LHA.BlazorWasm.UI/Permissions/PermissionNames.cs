namespace LHA.BlazorWasm.UI.Permissions;

/// <summary>
/// Strongly-typed permission name constants.
/// Modules can define their own permission sets extending this pattern.
/// </summary>
public static class PermissionNames
{
    // ── Dashboard ─────────────────────────────────────────────────────
    public const string DashboardView = "Dashboard.View";
    public const string DashboardManage = "Dashboard.Manage";

    // ── Users ─────────────────────────────────────────────────────────
    public const string UsersView = "Users.View";
    public const string UsersCreate = "Users.Create";
    public const string UsersEdit = "Users.Edit";
    public const string UsersDelete = "Users.Delete";
    public const string UsersManage = "Users.Manage";

    // ── Roles ─────────────────────────────────────────────────────────
    public const string RolesView = "Roles.View";
    public const string RolesManage = "Roles.Manage";

    // ── Tenants ───────────────────────────────────────────────────────
    public const string TenantsView = "Tenants.View";
    public const string TenantsManage = "Tenants.Manage";
    public const string TenantSwitch = "Tenants.Switch";

    // ── Settings ──────────────────────────────────────────────────────
    public const string SettingsView = "Settings.View";
    public const string SettingsManage = "Settings.Manage";

    // ── Administration ────────────────────────────────────────────────
    public const string AdminAccess = "Admin.Access";
    public const string AdminAuditLog = "Admin.AuditLog";
    public const string AdminSystem = "Admin.System";

    /// <summary>
    /// Returns all defined permission names.
    /// </summary>
    public static IReadOnlyList<string> All => new[]
    {
        DashboardView, DashboardManage,
        UsersView, UsersCreate, UsersEdit, UsersDelete, UsersManage,
        RolesView, RolesManage,
        TenantsView, TenantsManage, TenantSwitch,
        SettingsView, SettingsManage,
        AdminAccess, AdminAuditLog, AdminSystem
    };
}

namespace LHA.Core.Users;

/// <summary>
/// Well-known identifiers for system-level user contexts.
/// <para>
/// These IDs are deterministic (version 5 UUID from a namespace UUID + name)
/// so that they are consistent across all environments, databases, and services.
/// </para>
/// <para>
/// Use these constants when performing system operations that require a
/// creator/modifier identity — e.g., database migrations, seed data,
/// background jobs, or scheduled tasks.
/// </para>
/// </summary>
public static class CurrentUserDefaults
{
    // ─── Namespace UUID for deterministic generation ─────────────
    // Using a fixed namespace ensures all services derive the same IDs.
    // Generated once: Guid.NewGuid() → fixed forever.

    /// <summary>
    /// The system user — used by migrations, seed operations, and internal
    /// platform processes that are not initiated by a human.
    /// <para>Seed this user in the database with UserName = "system".</para>
    /// </summary>
    public static readonly Guid SystemUserId =
        new("00000000-0000-0000-0000-000000000001");

    /// <summary>
    /// The system user name.
    /// </summary>
    public const string SystemUserName = "system";

    /// <summary>
    /// The system user email.
    /// </summary>
    public const string SystemUserEmail = "system@internal";

    /// <summary>
    /// The default admin user — used during initial bootstrapping.
    /// <para>Seed this user in the database with UserName = "admin".</para>
    /// </summary>
    public static readonly Guid AdminUserId =
        new("00000000-0000-0000-0000-000000000002");

    /// <summary>
    /// The admin user name.
    /// </summary>
    public const string AdminUserName = "admin";

    /// <summary>
    /// The admin user email.
    /// </summary>
    public const string AdminUserEmail = "admin@lienhoaapp.com";

    /// <summary>
    /// The admin user default password (only used during seed, should be changed).
    /// </summary>
    public const string AdminUserDefaultPassword = "Admin@123456";

    /// <summary>
    /// Represents an anonymous (unauthenticated) context.
    /// When an operation happens without authentication, this ID may appear
    /// in audit logs to distinguish from a missing value.
    /// </summary>
    public static readonly Guid AnonymousUserId =
        new("00000000-0000-0000-0000-000000000000");

    /// <summary>
    /// The anonymous user name.
    /// </summary>
    public const string AnonymousUserName = "anonymous";

    // ─── Default Tenant ─────────────────────────────────────────

    /// <summary>
    /// The default tenant ID — the first tenant created during seed.
    /// </summary>
    public static readonly Guid DefaultTenantId =
        new("00000000-0000-0000-0001-000000000001");

    /// <summary>
    /// The default tenant name.
    /// </summary>
    public const string DefaultTenantName = "Default";

    // ─── Default Role ───────────────────────────────────────────

    /// <summary>
    /// The default admin role ID.
    /// </summary>
    public static readonly Guid AdminRoleId =
        new("00000000-0000-0000-0002-000000000001");

    /// <summary>
    /// The default admin role name. (Deprecated: use SystemSuperAdminRoleName or TenantAdminRoleName)
    /// </summary>
    [Obsolete("Use SystemSuperAdminRoleName or TenantAdminRoleName instead.")]
    public const string AdminRoleName = "admin";

    /// <summary>
    /// The system super admin role name (Host context).
    /// </summary>
    public const string SystemSuperAdminRoleName = "SystemSuperAdmin";

    /// <summary>
    /// The tenant admin role name.
    /// </summary>
    public const string TenantAdminRoleName = "TenantAdmin";

    // ─── Helper methods ─────────────────────────────────────────

    /// <summary>
    /// Returns <c>true</c> if the given <paramref name="userId"/>
    /// represents a well-known system identity (not a real human user).
    /// </summary>
    public static bool IsSystemIdentity(Guid? userId) =>
        userId == SystemUserId || userId == AnonymousUserId;

    /// <summary>
    /// Returns <c>true</c> if the given <paramref name="userId"/>
    /// is the built-in admin account.
    /// </summary>
    public static bool IsAdminUser(Guid? userId) =>
        userId == AdminUserId;
}

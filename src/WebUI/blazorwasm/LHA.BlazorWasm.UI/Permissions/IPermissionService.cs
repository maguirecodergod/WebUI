namespace LHA.BlazorWasm.UI.Permissions;

/// <summary>
/// Abstraction for checking user permissions.
/// Integrates with the navigation system to filter menu items automatically.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Check if the current user has a specific permission.
    /// </summary>
    Task<bool> HasPermissionAsync(string permission);

    /// <summary>
    /// Check if the current user has ALL of the specified permissions.
    /// </summary>
    Task<bool> HasAllPermissionsAsync(params string[] permissions);

    /// <summary>
    /// Check if the current user has ANY of the specified permissions.
    /// </summary>
    Task<bool> HasAnyPermissionAsync(params string[] permissions);

    /// <summary>
    /// Get all permissions for the current user.
    /// </summary>
    Task<IReadOnlySet<string>> GetCurrentPermissionsAsync();

    /// <summary>
    /// Event raised when permissions change (e.g., after role update or tenant switch).
    /// </summary>
    event Action? OnPermissionsChanged;
}

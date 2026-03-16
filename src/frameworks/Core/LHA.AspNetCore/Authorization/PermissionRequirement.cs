using Microsoft.AspNetCore.Authorization;

namespace LHA.AspNetCore.Authorization;

/// <summary>
/// Authorization requirement that demands a specific permission claim in the JWT.
/// </summary>
public sealed class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>The permission name that must be present in the user's <c>permissions</c> claim.</summary>
    public string PermissionName { get; }

    public PermissionRequirement(string permissionName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permissionName);
        PermissionName = permissionName;
    }
}

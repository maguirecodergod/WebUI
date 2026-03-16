namespace LHA.BlazorWasm.UI.Permissions;

/// <summary>
/// Default permission service implementation.
/// In production, this would integrate with an identity provider.
/// </summary>
public sealed class PermissionService : IPermissionService
{
    private HashSet<string> _permissions = new(StringComparer.OrdinalIgnoreCase);

    public event Action? OnPermissionsChanged;

    public Task<bool> HasPermissionAsync(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            return Task.FromResult(true);

        return Task.FromResult(_permissions.Contains(permission));
    }

    public async Task<bool> HasAllPermissionsAsync(params string[] permissions)
    {
        foreach (var permission in permissions)
        {
            if (!await HasPermissionAsync(permission))
                return false;
        }
        return true;
    }

    public async Task<bool> HasAnyPermissionAsync(params string[] permissions)
    {
        foreach (var permission in permissions)
        {
            if (await HasPermissionAsync(permission))
                return true;
        }
        return false;
    }

    public Task<IReadOnlySet<string>> GetCurrentPermissionsAsync()
    {
        return Task.FromResult<IReadOnlySet<string>>(_permissions);
    }

    /// <summary>
    /// Sets the current permission set. Called after login or role change.
    /// </summary>
    public void SetPermissions(IEnumerable<string> permissions)
    {
        _permissions = new HashSet<string>(permissions, StringComparer.OrdinalIgnoreCase);
        OnPermissionsChanged?.Invoke();
    }

    /// <summary>
    /// Grants all permissions (for development/testing).
    /// </summary>
    public void GrantAll()
    {
        _permissions = new HashSet<string>(PermissionNames.All, StringComparer.OrdinalIgnoreCase);
        OnPermissionsChanged?.Invoke();
    }
}

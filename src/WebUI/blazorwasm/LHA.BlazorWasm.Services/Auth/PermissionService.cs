using System.Security.Claims;

namespace LHA.BlazorWasm.Services.Auth;

public interface IPermissionService
{
    void SetUser(ClaimsPrincipal? user);
    void SetPermissions(IEnumerable<string> permissions);
    bool HasPermission(string permission);
    bool HasRole(string role);
}

public class PermissionService : IPermissionService
{
    private ClaimsPrincipal? _user;
    private HashSet<string> _permissions = new(StringComparer.OrdinalIgnoreCase);

    public void SetUser(ClaimsPrincipal? user)
    {
        _user = user;
        if (user == null)
        {
            _permissions.Clear();
        }
    }

    public void SetPermissions(IEnumerable<string> permissions)
    {
        _permissions = new HashSet<string>(permissions, StringComparer.OrdinalIgnoreCase);
    }

    public bool HasPermission(string permission)
    {
        if (_user?.Identity?.IsAuthenticated != true)
            return false;

        // Check explicit permissions list first
        if (_permissions.Contains(permission))
            return true;

        // Fallback to check using default permission claim type
        return _user.HasClaim(c => c.Type == "permissions" && string.Equals(c.Value, permission, StringComparison.OrdinalIgnoreCase));
    }

    public bool HasRole(string role)
    {
        if (_user?.Identity?.IsAuthenticated != true)
            return false;

        // Check using default role claim type
        return _user.HasClaim(c => (c.Type == "role" || c.Type == ClaimTypes.Role) && string.Equals(c.Value, role, StringComparison.OrdinalIgnoreCase));
    }
}

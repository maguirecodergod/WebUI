namespace LHA.PermissionManagement.Application.Contracts;

// ─── Integration Events (ETOs) ──────────────────────────────────────────

/// <summary>Published when permission definitions are registered for a service.</summary>
public sealed record PermissionDefinitionsRegisteredEto(string ServiceName, List<string> PermissionNames);

/// <summary>Published when a permission is granted.</summary>
public sealed record PermissionGrantedEto(
    string PermissionName, string ProviderName, string ProviderKey, Guid? TenantId);

/// <summary>Published when a permission grant is revoked.</summary>
public sealed record PermissionRevokedEto(
    string PermissionName, string ProviderName, string ProviderKey, Guid? TenantId);

using LHA.Ddd.Domain;

namespace LHA.PermissionManagement.Domain.PermissionGrants;

/// <summary>Raised when a permission is granted.</summary>
public sealed record PermissionGrantedDomainEvent(
    string PermissionName, string ProviderName, string ProviderKey, Guid? TenantId) : IDomainEvent;

/// <summary>Raised when a permission grant is revoked.</summary>
public sealed record PermissionRevokedDomainEvent(
    string PermissionName, string ProviderName, string ProviderKey, Guid? TenantId) : IDomainEvent;

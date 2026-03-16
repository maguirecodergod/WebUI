using LHA.Ddd.Domain;

namespace LHA.PermissionManagement.Domain;

// ─── PermissionDefinition Events ─────────────────────────────────

/// <summary>Raised when a permission definition is registered.</summary>
public sealed record PermissionDefinitionCreatedDomainEvent(
    Guid PermissionDefinitionId, string Name, string ServiceName) : IDomainEvent;

// ─── PermissionGroup Events ──────────────────────────────────────

/// <summary>Raised when a permission group is created.</summary>
public sealed record PermissionGroupCreatedDomainEvent(
    Guid PermissionGroupId, string Name) : IDomainEvent;

/// <summary>Raised when permissions in a group are synced.</summary>
public sealed record PermissionGroupSyncedDomainEvent(
    Guid PermissionGroupId) : IDomainEvent;

// ─── PermissionTemplate Events ───────────────────────────────────

/// <summary>Raised when a permission template is created.</summary>
public sealed record PermissionTemplateCreatedDomainEvent(
    Guid PermissionTemplateId, string Name) : IDomainEvent;

/// <summary>Raised when groups in a template are synced.</summary>
public sealed record PermissionTemplateSyncedDomainEvent(
    Guid PermissionTemplateId) : IDomainEvent;

// ─── PermissionGrant Events ──────────────────────────────────────

/// <summary>Raised when a permission is granted.</summary>
public sealed record PermissionGrantedDomainEvent(
    string PermissionName, string ProviderName, string ProviderKey, Guid? TenantId) : IDomainEvent;

/// <summary>Raised when a permission grant is revoked.</summary>
public sealed record PermissionRevokedDomainEvent(
    string PermissionName, string ProviderName, string ProviderKey, Guid? TenantId) : IDomainEvent;

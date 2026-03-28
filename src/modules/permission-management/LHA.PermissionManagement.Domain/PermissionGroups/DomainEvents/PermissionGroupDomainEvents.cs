using LHA.Ddd.Domain;

namespace LHA.PermissionManagement.Domain.PermissionGroups;

/// <summary>Raised when a permission group is created.</summary>
public sealed record PermissionGroupCreatedDomainEvent(
    Guid PermissionGroupId, string Name) : IDomainEvent;

/// <summary>Raised when permissions in a group are synced.</summary>
public sealed record PermissionGroupSyncedDomainEvent(
    Guid PermissionGroupId) : IDomainEvent;

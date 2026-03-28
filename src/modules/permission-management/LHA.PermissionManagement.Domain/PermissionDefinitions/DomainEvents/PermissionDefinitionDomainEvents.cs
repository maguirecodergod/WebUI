using LHA.Ddd.Domain;

namespace LHA.PermissionManagement.Domain.PermissionDefinitions;

/// <summary>Raised when a permission definition is registered.</summary>
public sealed record PermissionDefinitionCreatedDomainEvent(
    Guid PermissionDefinitionId, string Name, string ServiceName) : IDomainEvent;

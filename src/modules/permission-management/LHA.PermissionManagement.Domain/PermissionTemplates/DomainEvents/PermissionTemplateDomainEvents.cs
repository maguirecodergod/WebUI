using LHA.Ddd.Domain;

namespace LHA.PermissionManagement.Domain.PermissionTemplates;

/// <summary>Raised when a permission template is created.</summary>
public sealed record PermissionTemplateCreatedDomainEvent(
    Guid PermissionTemplateId, string Name) : IDomainEvent;

/// <summary>Raised when groups in a template are synced.</summary>
public sealed record PermissionTemplateSyncedDomainEvent(
    Guid PermissionTemplateId) : IDomainEvent;

using LHA.Ddd.Domain;

namespace LHA.TenantManagement.Domain;

// ─── Domain Events ───────────────────────────────────────────────────────

/// <summary>Raised when a new tenant is created.</summary>
public sealed record TenantCreatedDomainEvent(Guid TenantId, string Name) : IDomainEvent;

/// <summary>Raised when a tenant's name is changed.</summary>
public sealed record TenantNameChangedDomainEvent(Guid TenantId, string OldName, string NewName) : IDomainEvent;

/// <summary>Raised when a tenant is activated or deactivated.</summary>
public sealed record TenantActivationChangedDomainEvent(Guid TenantId, bool IsActive) : IDomainEvent;

/// <summary>Raised when a connection string is added, updated, or removed.</summary>
public sealed record TenantConnectionStringChangedDomainEvent(Guid TenantId, string ConnectionStringName) : IDomainEvent;

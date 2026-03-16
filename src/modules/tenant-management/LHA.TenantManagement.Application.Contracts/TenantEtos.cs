namespace LHA.TenantManagement.Application.Contracts;

// ─── Integration Events (ETOs) ──────────────────────────────────────────
// These are published via the EventBus (outbox → transport) for
// consumption by other bounded contexts or external services.

/// <summary>Published when a new tenant is created.</summary>
public sealed record TenantCreatedEto(Guid TenantId, string Name, DateTimeOffset CreationTime);

/// <summary>Published when a tenant name changes.</summary>
public sealed record TenantNameChangedEto(Guid TenantId, string OldName, string NewName);

/// <summary>Published when a tenant is activated or deactivated.</summary>
public sealed record TenantActivationChangedEto(Guid TenantId, bool IsActive);

/// <summary>Published when a tenant's connection strings are modified.</summary>
public sealed record TenantConnectionStringChangedEto(Guid TenantId, string ConnectionStringName);

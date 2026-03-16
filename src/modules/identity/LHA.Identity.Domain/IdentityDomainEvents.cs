using LHA.Ddd.Domain;

namespace LHA.Identity.Domain;

// ─── User Events ─────────────────────────────────────────────────────

/// <summary>Raised when a new user is created.</summary>
public sealed record UserCreatedDomainEvent(
    Guid UserId, string UserName, string Email, Guid? TenantId) : IDomainEvent;

/// <summary>Raised when a user's role membership changes.</summary>
public sealed record UserRoleChangedDomainEvent(
    Guid UserId, Guid RoleId, bool IsAdded) : IDomainEvent;

/// <summary>Raised when a user is locked out.</summary>
public sealed record UserLockedOutDomainEvent(
    Guid UserId, DateTimeOffset LockoutEnd) : IDomainEvent;

/// <summary>Raised when a user's password is changed.</summary>
public sealed record UserPasswordChangedDomainEvent(Guid UserId) : IDomainEvent;

// ─── Role Events ─────────────────────────────────────────────────────

/// <summary>Raised when a new role is created.</summary>
public sealed record RoleCreatedDomainEvent(
    Guid RoleId, string Name, Guid? TenantId) : IDomainEvent;

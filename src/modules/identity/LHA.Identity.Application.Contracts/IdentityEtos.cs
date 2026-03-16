namespace LHA.Identity.Application.Contracts;

// ─── User ETOs (Event Transfer Objects) ──────────────────────────────

/// <summary>Published when a new user is created.</summary>
public sealed record UserCreatedEto(Guid UserId, string UserName, string Email, Guid? TenantId, DateTimeOffset CreationTime);

/// <summary>Published when a user's role membership changes.</summary>
public sealed record UserRoleChangedEto(Guid UserId, Guid RoleId, bool IsAdded);

/// <summary>Published when a user is locked out.</summary>
public sealed record UserLockedOutEto(Guid UserId, DateTimeOffset LockoutEnd);

/// <summary>Published when login succeeds.</summary>
public sealed record LoginSucceededEto(Guid UserId, string UserName, Guid? TenantId, DateTimeOffset Timestamp);

/// <summary>Published when login fails.</summary>
public sealed record LoginFailedEto(string UserNameOrEmail, string Reason, Guid? TenantId, DateTimeOffset Timestamp);

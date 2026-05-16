using LHA.EventBus;

namespace LHA.Identity.Application.Contracts;

// ─── User ETOs (Event Transfer Objects) ──────────────────────────────

/// <summary>Published when a new user is created.</summary>
[EventName("LHA.Identity.Application.Contracts.UserCreated")]
public sealed record UserCreatedEto(Guid UserId, string UserName, string Email, DateTimeOffset CreationTime)
    : IntegrationEvent
{
    public UserCreatedEto() : this(Guid.Empty, string.Empty, string.Empty, DateTimeOffset.UtcNow) { }
}

/// <summary>Published when a user's role membership changes.</summary>
[EventName("LHA.Identity.Application.Contracts.UserRoleChanged")]
public sealed record UserRoleChangedEto(Guid UserId, Guid RoleId, bool IsAdded)
    : IntegrationEvent;

/// <summary>Published when a user is locked out.</summary>
[EventName("LHA.Identity.Application.Contracts.UserLockedOut")]
public sealed record UserLockedOutEto(Guid UserId, DateTimeOffset LockoutEnd)
    : IntegrationEvent;

/// <summary>Published when login succeeds.</summary>
[EventName("LHA.Identity.Application.Contracts.LoginSucceeded")]
public sealed record LoginSucceededEto(Guid UserId, string UserName)
    : IntegrationEvent
{
    public override string? PartitionKey => "LHA.Identity.Application.Contracts.LoginSucceeded";
}

/// <summary>Published when login fails.</summary>
[EventName("LHA.Identity.Application.Contracts.LoginFailed")]
public sealed record LoginFailedEto(string UserNameOrEmail, string Reason)
    : IntegrationEvent;

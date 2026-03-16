using LHA.EventBus;
using LHA.Identity.Application.Contracts;

namespace LHA.Identity.Consumer;

/// <summary>
/// Handles <see cref="UserCreatedEto"/> integration events.
/// </summary>
public sealed class UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger)
    : IEventHandler<UserCreatedEto>
{
    public Task HandleAsync(UserCreatedEto @event, EventContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "User created: {UserId} — UserName: {UserName}, Email: {Email}, TenantId: {TenantId}",
            @event.UserId, @event.UserName, @event.Email, @event.TenantId);

        // TODO: Send welcome email, provision user resources, etc.
        return Task.CompletedTask;
    }
}

/// <summary>
/// Handles <see cref="UserRoleChangedEto"/> integration events.
/// </summary>
public sealed class UserRoleChangedEventHandler(ILogger<UserRoleChangedEventHandler> logger)
    : IEventHandler<UserRoleChangedEto>
{
    public Task HandleAsync(UserRoleChangedEto @event, EventContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "User {UserId} role {Action}: RoleId={RoleId}",
            @event.UserId, @event.IsAdded ? "added" : "removed", @event.RoleId);

        // TODO: Invalidate permission caches, notify downstream services, etc.
        return Task.CompletedTask;
    }
}

/// <summary>
/// Handles <see cref="LoginFailedEto"/> integration events.
/// </summary>
public sealed class LoginFailedEventHandler(ILogger<LoginFailedEventHandler> logger)
    : IEventHandler<LoginFailedEto>
{
    public Task HandleAsync(LoginFailedEto @event, EventContext context, CancellationToken cancellationToken = default)
    {
        logger.LogWarning(
            "Login failed for '{UserNameOrEmail}': {Reason}, TenantId: {TenantId}",
            @event.UserNameOrEmail, @event.Reason, @event.TenantId);

        // TODO: Trigger brute-force protection, send alert, etc.
        return Task.CompletedTask;
    }
}

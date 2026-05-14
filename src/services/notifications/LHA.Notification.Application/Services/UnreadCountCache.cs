using LHA.Ddd.Application;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.Repositories;

namespace LHA.Notification.Application.Services;

/// <summary>
/// Implementation of <see cref="IUnreadCountCache"/> using the repository as the source of truth.
/// In a production environment, this would be backed by Redis or another distributed cache.
/// </summary>
public sealed class UnreadCountCache : ApplicationService, IUnreadCountCache
{
    private readonly INotificationRepository _notificationRepository;

    public UnreadCountCache(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    /// <inheritdoc />
    public async Task<int> GetUnreadCountAsync(Guid tenantId, Guid recipientId, CancellationToken cancellationToken = default)
    {
        return await _notificationRepository.GetUnreadCountByRecipientAsync(recipientId, cancellationToken);
    }

    /// <inheritdoc />
    public Task SetUnreadCountAsync(Guid tenantId, Guid recipientId, int count, CancellationToken cancellationToken = default)
    {
        // No-op for now, source of truth is the DB
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task IncrementUnreadCountAsync(Guid tenantId, Guid recipientId, CancellationToken cancellationToken = default)
    {
        // No-op, source of truth is the DB
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DecrementUnreadCountAsync(Guid tenantId, Guid recipientId, CancellationToken cancellationToken = default)
    {
        // No-op, source of truth is the DB
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ClearUnreadCountAsync(Guid tenantId, Guid recipientId, CancellationToken cancellationToken = default)
    {
        // No-op, source of truth is the DB
        return Task.CompletedTask;
    }
}

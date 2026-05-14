namespace LHA.Notification.Application.Contracts;

public interface IUnreadCountCache
{
    Task<int> GetUnreadCountAsync(Guid tenantId, Guid recipientId, CancellationToken cancellationToken = default);
    Task SetUnreadCountAsync(Guid tenantId, Guid recipientId, int count, CancellationToken cancellationToken = default);
    Task IncrementUnreadCountAsync(Guid tenantId, Guid recipientId, CancellationToken cancellationToken = default);
    Task DecrementUnreadCountAsync(Guid tenantId, Guid recipientId, CancellationToken cancellationToken = default);
    Task ClearUnreadCountAsync(Guid tenantId, Guid recipientId, CancellationToken cancellationToken = default);
}
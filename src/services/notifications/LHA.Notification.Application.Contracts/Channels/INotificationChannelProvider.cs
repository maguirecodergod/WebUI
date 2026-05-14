using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public interface INotificationChannelProvider
{
    CNotificationChannel Channel { get; }
    Task<bool> ValidateRecipientAsync(Guid recipientId, Guid tenantId, CancellationToken cancellationToken = default);
    Task SendAsync(Guid recipientId, string subject, string body, Dictionary<string, string>? metadata, Dictionary<string, object>? variables, CancellationToken cancellationToken = default);
    Task<bool> SendBatchAsync(IEnumerable<Guid> recipientIds, string subject, string body, Dictionary<string, string>? metadata, Dictionary<string, object>? variables, CancellationToken cancellationToken = default);
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
    Task<string> GetStatusAsync(CancellationToken cancellationToken = default);
}
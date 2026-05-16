using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public interface IInAppProvider : INotificationChannelProvider
{
    Task<bool> SendInAppAsync(string recipientId, Guid tenantId, string title, string body, string? actionUrl = null, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default);
    Task<bool> SendTemplateInAppAsync(string recipientId, Guid tenantId, Guid templateId, Dictionary<string, object>? variables = null, CancellationToken cancellationToken = default);
    Task<bool> SendBulkInAppAsync(IEnumerable<string> recipientIds, Guid tenantId, string title, string body, string? actionUrl = null, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteInAppAsync(Guid notificationId, string recipientId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> MarkAsReadAsync(Guid notificationId, string recipientId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(string recipientId, Guid tenantId, CancellationToken cancellationToken = default);
}

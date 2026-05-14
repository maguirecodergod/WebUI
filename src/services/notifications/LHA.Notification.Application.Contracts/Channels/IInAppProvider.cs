namespace LHA.Notification.Application.Contracts;

public interface IInAppProvider : INotificationChannelProvider
{
    Task<bool> SendInAppAsync(Guid recipientId, string title, string body, string? actionUrl, Dictionary<string, string>? data, CancellationToken cancellationToken = default);
    Task<bool> SendTemplateInAppAsync(Guid recipientId, string templateId, Dictionary<string, object>? variables, CancellationToken cancellationToken = default);
    Task<bool> SendBulkInAppAsync(IEnumerable<Guid> recipientIds, string title, string body, string? actionUrl, Dictionary<string, string>? data, CancellationToken cancellationToken = default);
    Task<bool> DeleteInAppAsync(Guid notificationId, Guid recipientId, CancellationToken cancellationToken = default);
    Task<bool> MarkAsReadAsync(Guid notificationId, Guid recipientId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid recipientId, CancellationToken cancellationToken = default);
}
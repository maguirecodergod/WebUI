using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public interface INotificationChannelProvider
{
    CNotificationChannel Channel { get; }
    
    Task<bool> ValidateRecipientAsync(string recipientId, Guid tenantId, CancellationToken cancellationToken = default);
    
    Task SendAsync(string recipientId, Guid tenantId, string subject, string body, Dictionary<string, string>? metadata = null, Dictionary<string, object>? variables = null, CancellationToken cancellationToken = default);
    
    Task<bool> SendBatchAsync(IEnumerable<string> recipientIds, Guid tenantId, string subject, string body, Dictionary<string, string>? metadata = null, Dictionary<string, object>? variables = null, CancellationToken cancellationToken = default);
    
    Task<bool> IsAvailableAsync(Guid tenantId, CancellationToken cancellationToken = default);
    
    Task<string> GetStatusAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

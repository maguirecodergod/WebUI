using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public interface IWebPushProvider : INotificationChannelProvider
{
    Task<bool> RegisterSubscriptionAsync(string endpoint, string keys, string userId, string deviceId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> UnregisterSubscriptionAsync(string endpoint, Guid tenantId, CancellationToken cancellationToken = default);
    Task<List<string>> GetInvalidSubscriptionsAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> ValidateSubscriptionAsync(string endpoint, Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> SendWebPushAsync(string endpoint, Guid tenantId, string title, string body, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default);
}

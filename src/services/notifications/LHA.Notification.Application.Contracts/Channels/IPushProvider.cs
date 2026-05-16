using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public interface IPushProvider : INotificationChannelProvider
{
    Task<bool> RegisterDeviceAsync(string deviceToken, string userId, string deviceId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> UnregisterDeviceAsync(string deviceToken, string userId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<List<string>> GetInvalidTokensAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string deviceToken, Guid tenantId, CancellationToken cancellationToken = default);
}

public interface IFcmPushProvider : IPushProvider { }
public interface IApnsPushProvider : IPushProvider { }

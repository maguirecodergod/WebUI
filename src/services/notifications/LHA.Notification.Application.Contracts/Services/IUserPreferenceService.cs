using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public interface IUserPreferenceService
{
    Task<UserPreferenceDto> GetAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<UserPreferenceDto> UpdateAsync(Guid userId, UpdatePreferenceDto request, Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> OptOutAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> OptInAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> IsChannelEnabledAsync(Guid userId, Guid tenantId, string channel, CancellationToken cancellationToken = default);
    Task<bool> IsCategoryEnabledAsync(Guid userId, Guid tenantId, string category, CancellationToken cancellationToken = default);
    Task<List<CNotificationChannel>> GetEnabledChannelsAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> IsQuietHourAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
}

using LHA.Ddd.Domain;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Domain.Repositories;

public interface IUserPreferenceRepository : IRepository<UserPreferenceEntity, Guid>
{
    Task<UserPreferenceEntity?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserPreferenceEntity>> GetByOptOutStatusAsync(bool optOut, CancellationToken cancellationToken = default);
    IAsyncEnumerable<UserPreferenceEntity> GetByTenantCursorAsync(string tenantId, int batchSize, CancellationToken cancellationToken = default);
    Task<bool> IsUserOptedOutAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsChannelEnabledAsync(Guid userId, CNotificationChannel channel, CancellationToken cancellationToken = default);
    Task<IEnumerable<CNotificationChannel>> GetEnabledChannelsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
using LHA.Ddd.Domain;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Domain.Repositories;

public interface IDeviceRepository : IRepository<DeviceEntity, Guid>
{
    Task<IEnumerable<DeviceEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<DeviceEntity?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task<IEnumerable<DeviceEntity>> GetInactiveDevicesAsync(DateTimeOffset lastSeenBefore, int limit, CancellationToken cancellationToken = default);
    Task<IEnumerable<DeviceEntity>> GetByPlatformAsync(CDevicePlatform platform, CancellationToken cancellationToken = default);
    IAsyncEnumerable<DeviceEntity> GetByTenantCursorAsync(int batchSize, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserIdAndPlatformAsync(Guid userId, CDevicePlatform platform, CancellationToken cancellationToken = default);
}
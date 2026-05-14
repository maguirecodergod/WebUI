using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public interface IDeviceService
{
    Task<DeviceDto> RegisterAsync(RegisterDeviceDto request, Guid tenantId, Guid userId, CancellationToken cancellationToken = default);
    Task<DeviceDto> UpdateAsync(Guid id, UpdateDeviceDto request, Guid tenantId, CancellationToken cancellationToken = default);
    Task<DeviceDto?> DeleteAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<DeviceDto?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<DeviceListDto> GetByUserIdAsync(Guid userId, Guid tenantId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<List<CNotificationChannel>> GetEnabledChannelsAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<List<DeviceDto>> GetInactiveDevicesAsync(Guid tenantId, DateTimeOffset lastSeenBefore, int limit, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string token, Guid tenantId, Guid userId, CancellationToken cancellationToken = default);
}
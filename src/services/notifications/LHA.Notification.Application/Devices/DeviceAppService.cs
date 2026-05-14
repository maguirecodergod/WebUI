using LHA.Ddd.Application;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain;
using LHA.Notification.Domain.DomainEvents;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Domain.Shared;
using LHA.UnitOfWork;

namespace LHA.Notification.Application.Devices;

public sealed class DeviceAppService : ApplicationService, IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IUnitOfWorkManager _uowManager;

    public DeviceAppService(
        IDeviceRepository deviceRepository,
        IUnitOfWorkManager uowManager)
    {
        _deviceRepository = deviceRepository;
        _uowManager = uowManager;
    }

    public async Task<DeviceDto> RegisterAsync(RegisterDeviceDto request, Guid tenantId, Guid userId, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var existing = await _deviceRepository.GetByTokenHashAsync(
            HashToken(request.Token), cancellationToken);

        if (existing is not null)
        {
            existing.Activate();
            existing.UpdateLastSeen(DateTime.UtcNow);
            await _deviceRepository.UpdateAsync(existing, cancellationToken);
            await uow.CompleteAsync();
            return MapToDto(existing);
        }

        var entity = new DeviceEntity(
            userId: userId,
            platform: request.Platform,
            token: request.Token,
            appVersion: request.AppVersion,
            osVersion: request.OsVersion,
            deviceModel: request.DeviceModel,
            locale: request.Locale,
            timezone: request.Timezone);

        await _deviceRepository.InsertAsync(entity, cancellationToken);
        await uow.CompleteAsync();

        return MapToDto(entity);
    }

    public async Task<DeviceDto> UpdateAsync(Guid id, UpdateDeviceDto request, Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = await _deviceRepository.GetAsync(id, cancellationToken);
        entity.UpdateLastSeen(DateTime.UtcNow);
        await _deviceRepository.UpdateAsync(entity, cancellationToken);
        await uow.CompleteAsync();

        return MapToDto(entity);
    }

    public async Task<DeviceDto?> DeleteAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = await _deviceRepository.FindAsync(id, cancellationToken);
        if (entity is null) return null;

        entity.Deactivate();
        await _deviceRepository.UpdateAsync(entity, cancellationToken);
        await uow.CompleteAsync();

        return MapToDto(entity);
    }

    public async Task<DeviceDto?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entity = await _deviceRepository.FindAsync(id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<DeviceListDto> GetByUserIdAsync(Guid userId, Guid tenantId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var all = await _deviceRepository.GetByUserIdAsync(userId, cancellationToken);
        var paged = all.Skip((page - 1) * pageSize).Take(pageSize).Select(MapToDto).ToList();
        return new DeviceListDto(paged, (int)all.Count(), page, pageSize, (page * pageSize) < all.Count());
    }

    public async Task<List<CNotificationChannel>> GetEnabledChannelsAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var devices = await _deviceRepository.GetByUserIdAsync(userId, cancellationToken);
        var channels = new List<CNotificationChannel>();

        if (devices.Any(d => d.IsActive && d.Platform == CDevicePlatform.Android))
            channels.Add(CNotificationChannel.Push);
        if (devices.Any(d => d.IsActive && d.Platform == CDevicePlatform.Ios))
            channels.Add(CNotificationChannel.Push);

        return channels.Distinct().ToList();
    }

    public async Task<List<DeviceDto>> GetInactiveDevicesAsync(Guid tenantId, DateTimeOffset lastSeenBefore, int limit, CancellationToken cancellationToken = default)
    {
        var entities = await _deviceRepository.GetInactiveDevicesAsync(lastSeenBefore, limit, cancellationToken);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<bool> ValidateTokenAsync(string token, Guid tenantId, Guid userId, CancellationToken cancellationToken = default)
    {
        var hash = HashToken(token);
        var entity = await _deviceRepository.GetByTokenHashAsync(hash, cancellationToken);
        return entity is not null && entity.UserId == userId && entity.IsActive;
    }

    // ─── Mapping ─────────────────────────────────────────────────────

    private static DeviceDto MapToDto(DeviceEntity e) => new(
        Id: e.Id,
        TenantId: e.TenantId ?? Guid.Empty,
        UserId: e.UserId,
        Platform: e.Platform,
        AppVersion: e.AppVersion,
        OsVersion: e.OsVersion,
        DeviceModel: e.DeviceModel,
        Locale: e.Locale,
        Timezone: e.Timezone,
        IsActive: e.IsActive,
        LastSeenAt: e.LastSeenAt,
        RegisteredAt: e.RegisteredAt);

    private static string HashToken(string token)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        return Convert.ToHexString(sha.ComputeHash(bytes));
    }
}

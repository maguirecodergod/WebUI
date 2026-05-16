using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record DeviceDto(
    Guid Id,
    Guid TenantId,
    Guid UserId,
    CDevicePlatform Platform,
    string AppVersion,
    string OsVersion,
    string DeviceModel,
    string Locale,
    string Timezone,
    bool IsActive,
    DateTimeOffset LastSeenAt,
    DateTimeOffset RegisteredAt);

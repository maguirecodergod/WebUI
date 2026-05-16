using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record UpdateDeviceDto(
    CDevicePlatform? Platform,
    string? AppVersion,
    string? OsVersion,
    string? DeviceModel,
    string? Locale,
    string? Timezone);

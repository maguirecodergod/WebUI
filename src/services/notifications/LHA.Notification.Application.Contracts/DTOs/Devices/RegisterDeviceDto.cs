using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record RegisterDeviceDto(
    CDevicePlatform Platform,
    string Token,
    string AppVersion,
    string OsVersion,
    string DeviceModel,
    string Locale,
    string Timezone);

using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record CategoryPreferenceDto(
    string Category,
    bool Enabled,
    List<CNotificationChannel> Channels);

using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;
public record ChannelPreferenceDto(
    CNotificationChannel Channel,
    bool Enabled,
    List<string> Categories);
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record CreateTemplateDto(
    string Code,
    string Name,
    string Description,
    CNotificationType Type,
    List<CNotificationChannel> SupportedChannels,
    string DefaultLocale,
    List<string>? Tags);

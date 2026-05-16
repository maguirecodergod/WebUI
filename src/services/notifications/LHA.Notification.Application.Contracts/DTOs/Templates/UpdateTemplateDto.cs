using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record UpdateTemplateDto(
    string? Name,
    string? Description,
    List<CNotificationChannel>? SupportedChannels,
    string? DefaultLocale,
    List<string>? Tags,
    bool? IsActive);

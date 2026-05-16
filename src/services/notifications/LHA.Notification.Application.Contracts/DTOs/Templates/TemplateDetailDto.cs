using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record TemplateDetailDto(
    Guid Id,
    Guid TenantId,
    string Code,
    string Name,
    string Description,
    CNotificationType Type,
    List<CNotificationChannel> SupportedChannels,
    bool IsActive,
    bool IsSystem,
    string DefaultLocale,
    List<string> Tags,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    List<TemplateVersionDto> Versions);

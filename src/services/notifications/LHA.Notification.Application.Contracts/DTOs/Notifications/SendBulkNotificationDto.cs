using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record SendBulkNotificationDto(
    string Name,
    List<SendNotificationDto> Recipients,
    Guid? TemplateId,
    Dictionary<string, object>? TemplateVariables,
    DateTimeOffset? ExpiresAt,
    List<CNotificationChannel>? Channels);

namespace LHA.Notification.Application.Contracts;

public record CreateBatchDto(
    string Name,
    Guid? TemplateId,
    List<BatchRecipientDto> Recipients,
    List<string>? Tags,
    DateTimeOffset? ScheduledAt);

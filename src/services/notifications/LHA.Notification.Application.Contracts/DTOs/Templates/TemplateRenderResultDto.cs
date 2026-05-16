using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record TemplateRenderResultDto(
    Guid TemplateId,
    string Code,
    CNotificationType Type,
    string? Subject,
    string Body,
    string? HtmlBody,
    string? PushTitle,
    string? PushBody,
    CTemplateEngineType EngineType);

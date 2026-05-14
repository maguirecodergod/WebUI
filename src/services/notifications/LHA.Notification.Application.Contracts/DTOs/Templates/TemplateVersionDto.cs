using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record TemplateVersionDto(
    int Version,
    string Locale,
    CNotificationChannel Channel,
    string? SubjectTemplate,
    string BodyTemplate,
    string? HtmlTemplate,
    string? PushTitle,
    string? PushBody,
    List<TemplateVariableDefinitionDto> Variables,
    bool IsDefault,
    DateTimeOffset CreatedAt);
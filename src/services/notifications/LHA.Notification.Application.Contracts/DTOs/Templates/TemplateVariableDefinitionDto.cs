using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record TemplateVariableDefinitionDto(
    string Name,
    CVariableType Type,
    bool Required,
    string? DefaultValue,
    string Description);
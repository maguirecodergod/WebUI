using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public interface ITemplateEngine
{
    string RenderSubject(string template, Dictionary<string, object>? variables, string? locale = null);
    string RenderBody(string template, Dictionary<string, object>? variables, string? locale = null);
    string RenderHtml(string template, Dictionary<string, object>? variables, string? locale = null);
    string RenderPushTitle(string template, Dictionary<string, object>? variables, string? locale = null);
    string RenderPushBody(string template, Dictionary<string, object>? variables, string? locale = null);
    CTemplateEngineType GetEngineType(string template);
    TemplateVariableDefinitionDto? ExtractVariables(string template);
}

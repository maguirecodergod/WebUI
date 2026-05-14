namespace LHA.Notification.Application.Contracts;

public interface ITemplateRenderer
{
    Task<TemplateRenderResultDto> RenderAsync(Guid templateId, Guid tenantId, Dictionary<string, object>? variables, string? locale = null, CancellationToken cancellationToken = default);
    Task<TemplateRenderResultDto> PreviewAsync(string bodyTemplate, string? subjectTemplate, string? htmlTemplate, Dictionary<string, object>? variables, string? locale = null, CancellationToken cancellationToken = default);
}

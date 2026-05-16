using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Infrastructure
{
    internal sealed class TemplateRenderer : ITemplateRenderer
    {
        private readonly ITemplateEngine _templateEngine;
        private readonly ITemplateRepository _templateRepository;

        public TemplateRenderer(ITemplateEngine templateEngine, ITemplateRepository templateRepository)
        {
            _templateEngine = templateEngine;
            _templateRepository = templateRepository;
        }

        public async Task<TemplateRenderResultDto> RenderAsync(Guid templateId, Guid tenantId, Dictionary<string, object>? variables, string? locale = null, CancellationToken cancellationToken = default)
        {
            var template = await _templateRepository.FindAsync(templateId, cancellationToken);
            if (template == null)
                throw new InvalidOperationException($"Template {templateId} not found");

            var version = template.Versions.FirstOrDefault(v => v.Locale == (locale ?? template.DefaultLocale))
                          ?? template.Versions.FirstOrDefault();

            if (version == null)
                throw new InvalidOperationException($"No version found for template {templateId}");

            var subject = version.SubjectTemplate != null ? _templateEngine.RenderSubject(version.SubjectTemplate, variables, locale) : null;
            var body = _templateEngine.RenderBody(version.BodyTemplate, variables, locale);
            var html = version.HtmlTemplate != null ? _templateEngine.RenderHtml(version.HtmlTemplate, variables, locale) : null;
            var pushTitle = version.PushTitle != null ? _templateEngine.RenderPushTitle(version.PushTitle, variables, locale) : null;
            var pushBody = version.PushBody != null ? _templateEngine.RenderPushBody(version.PushBody, variables, locale) : null;

            return new TemplateRenderResultDto(
                templateId,
                template.Code,
                template.Type,
                subject,
                body,
                html,
                pushTitle,
                pushBody,
                _templateEngine.GetEngineType(version.BodyTemplate));
        }

        public Task<TemplateRenderResultDto> PreviewAsync(string bodyTemplate, string? subjectTemplate, string? htmlTemplate, Dictionary<string, object>? variables, string? locale = null, CancellationToken cancellationToken = default)
        {
            var subject = subjectTemplate != null ? _templateEngine.RenderSubject(subjectTemplate, variables, locale) : null;
            var body = _templateEngine.RenderBody(bodyTemplate, variables, locale);
            var html = htmlTemplate != null ? _templateEngine.RenderHtml(htmlTemplate, variables, locale) : null;

            return Task.FromResult(new TemplateRenderResultDto(
                Guid.NewGuid(),
                "preview",
                CNotificationType.System,
                subject,
                body,
                html,
                null,
                null,
                CTemplateEngineType.Handlebars));
        }
    }
}

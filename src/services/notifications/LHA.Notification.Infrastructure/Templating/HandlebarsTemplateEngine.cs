using System.Text.RegularExpressions;
using LHA.Notification.Domain.Shared;
using LHA.Notification.Application.Contracts;

namespace LHA.Notification.Infrastructure
{
    internal sealed class HandlebarsTemplateEngine : ITemplateEngine
    {
        public string RenderSubject(string template, Dictionary<string, object>? variables, string? locale = null)
        {
            return RenderTemplate(template, variables);
        }

        public string RenderBody(string template, Dictionary<string, object>? variables, string? locale = null)
        {
            return RenderTemplate(template, variables);
        }

        public string RenderHtml(string template, Dictionary<string, object>? variables, string? locale = null)
        {
            return RenderTemplate(template, variables);
        }

        public string RenderPushTitle(string template, Dictionary<string, object>? variables, string? locale = null)
        {
            return RenderTemplate(template, variables);
        }

        public string RenderPushBody(string template, Dictionary<string, object>? variables, string? locale = null)
        {
            return RenderTemplate(template, variables);
        }

        public CTemplateEngineType GetEngineType(string template)
        {
            return CTemplateEngineType.Handlebars;
        }

        public TemplateVariableDefinitionDto? ExtractVariables(string template)
        {
            return null;
        }

        private static string RenderTemplate(string template, Dictionary<string, object>? variables)
        {
            if (string.IsNullOrEmpty(template) || variables == null)
                return template;

            var result = template;
            foreach (var kvp in variables)
            {
                result = Regex.Replace(result, @"\{\{\s*" + Regex.Escape(kvp.Key) + @"\s*\}\}", kvp.Value?.ToString() ?? string.Empty);
            }
            return result;
        }
    }
}

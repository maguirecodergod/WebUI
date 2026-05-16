using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Infrastructure
{
    internal sealed class LiquidTemplateEngine : ITemplateEngine
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
            return CTemplateEngineType.Liquid;
        }

        public TemplateVariableDefinitionDto? ExtractVariables(string template)
        {
            return null;
        }

        private static string RenderTemplate(string template, Dictionary<string, object>? variables)
        {
            if (variables == null || variables.Count == 0) return template;

            string result = template;
            foreach (KeyValuePair<string, object> variable in variables)
            {
                result = result.Replace($"{{{{{variable.Key}}}}}", variable.Value?.ToString() ?? string.Empty);
                result = result.Replace($"{{{{ {variable.Key} }}}}", variable.Value?.ToString() ?? string.Empty);
            }
            return result;
        }
    }
}

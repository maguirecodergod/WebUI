using LHA.Ddd.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.AspNetCore;

/// <summary>
/// A global MVC result filter that automatically enriches the DTO responses with Auditor (User) information.
/// It intercepts all ObjectResult responses, extracts the value, and passes it to <see cref="IAuditedDtoEnricher"/>.
/// </summary>
public class AuditEnrichmentResultFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult && objectResult.Value != null)
        {
            var enricher = context.HttpContext.RequestServices.GetService<IAuditedDtoEnricher>();
            if (enricher != null)
            {
                await enricher.EnrichAsync(objectResult.Value);
            }
        }

        await next();
    }
}

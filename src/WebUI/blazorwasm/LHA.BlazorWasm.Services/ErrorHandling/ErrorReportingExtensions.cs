using Microsoft.Extensions.DependencyInjection;

namespace LHA.BlazorWasm.Services.ErrorHandling;

/// <summary>
/// DI extensions for registering error reporting infrastructure.
/// </summary>
public static class ErrorReportingExtensions
{
    /// <summary>
    /// Adds the <see cref="IErrorReporter"/> service to the DI container.
    /// </summary>
    public static IServiceCollection AddErrorReporting(this IServiceCollection services)
    {
        services.AddScoped<IErrorReporter, ErrorReporter>();
        return services;
    }
}

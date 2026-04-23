using Microsoft.Extensions.DependencyInjection;

namespace LHA.Ddd.Application;

/// <summary>
/// Base class for application services. Provides common infrastructure such as
/// the service provider for resolving dependencies.
/// </summary>
public abstract class ApplicationService : IApplicationService
{
    /// <summary>
    /// The service provider for resolving dependencies within the application service.
    /// </summary>
    public required IServiceProvider ServiceProvider { protected get; init; }

    /// <summary>
    /// Enriches the given DTO or collection of DTOs with auditor information (Name, Avatar, Email).
    /// </summary>
    protected virtual async Task<T> EnrichAuditAsync<T>(T dto)
    {
        var enricher = ServiceProvider.GetService<IAuditedDtoEnricher>();
        if (enricher != null)
        {
            await enricher.EnrichAsync(dto);
        }
        return dto;
    }
}

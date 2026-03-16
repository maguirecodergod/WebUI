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
}

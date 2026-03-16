using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LHA.Ddd.Domain;

/// <summary>
/// Base class for domain services. Provides access to <see cref="IServiceProvider"/>
/// and a typed <see cref="ILogger"/>.
/// </summary>
/// <remarks>
/// Domain services encapsulate domain logic that spans multiple aggregates or
/// does not naturally belong to any single entity. Inject dependencies through
/// the constructor; use <see cref="ServiceProvider"/> only for optional / lazy resolution.
/// </remarks>
public abstract class DomainService : IDomainService
{
    private ILoggerFactory? _loggerFactory;
    private ILogger? _logger;

    /// <summary>
    /// The service provider for resolving dependencies within the domain service.
    /// Prefer constructor injection; use this only for deferred or optional resolution.
    /// </summary>
    public required IServiceProvider ServiceProvider { protected get; init; }

    /// <summary>
    /// Logger factory resolved from <see cref="ServiceProvider"/>.
    /// </summary>
    protected ILoggerFactory LoggerFactory =>
        _loggerFactory ??= ServiceProvider.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;

    /// <summary>
    /// Typed logger for the current domain service.
    /// </summary>
    protected ILogger Logger =>
        _logger ??= LoggerFactory.CreateLogger(GetType());
}

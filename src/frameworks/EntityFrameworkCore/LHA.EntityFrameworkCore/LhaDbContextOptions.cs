using Microsoft.EntityFrameworkCore;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// Centralized options for configuring <see cref="DbContext"/> instances
/// registered via <c>AddLhaDbContext</c>.
/// </summary>
public sealed class LhaDbContextOptions
{
    /// <summary>
    /// Default configuration action applied to every <see cref="LhaDbContextConfigurationContext"/>.
    /// Set via provider-specific extensions (e.g. <c>UseNpgsql</c>).
    /// </summary>
    public Action<LhaDbContextConfigurationContext>? DefaultConfigureAction { get; set; }

    /// <summary>
    /// Per-DbContext-type configuration actions that override the default.
    /// </summary>
    internal Dictionary<Type, Action<LhaDbContextConfigurationContext>> ConfigureActions { get; } = [];

    /// <summary>
    /// Maps a module DbContext type to its replacement (unified) DbContext type.
    /// Used by <see cref="UnitOfWorkDbContextProvider{TDbContext}"/> to resolve the correct
    /// concrete DbContext at runtime.
    /// </summary>
    internal Dictionary<Type, Type> DbContextReplacements { get; } = [];

    /// <summary>
    /// Configures all DbContext types with the specified action.
    /// </summary>
    public void Configure(Action<LhaDbContextConfigurationContext> configureAction)
    {
        ArgumentNullException.ThrowIfNull(configureAction);
        DefaultConfigureAction = configureAction;
    }

    /// <summary>
    /// Configures a specific DbContext type with a dedicated action.
    /// </summary>
    public void Configure<TDbContext>(Action<LhaDbContextConfigurationContext> configureAction)
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(configureAction);
        ConfigureActions[typeof(TDbContext)] = configureAction;
    }

    /// <summary>
    /// Registers a replacement so that when <typeparamref name="TOriginal"/> is requested,
    /// <typeparamref name="TReplacement"/> is resolved instead. All replaced DbContexts share
    /// the same instance within a Unit of Work.
    /// </summary>
    /// <typeparam name="TOriginal">The module's DbContext type to replace.</typeparam>
    /// <typeparam name="TReplacement">The unified DbContext type to use instead.</typeparam>
    public void ReplaceDbContext<TOriginal, TReplacement>()
        where TOriginal : DbContext
        where TReplacement : DbContext
    {
        DbContextReplacements[typeof(TOriginal)] = typeof(TReplacement);
    }
}

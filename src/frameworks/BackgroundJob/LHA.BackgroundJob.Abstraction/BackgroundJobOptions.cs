using System.Collections.Immutable;

namespace LHA.BackgroundJob;

/// <summary>
/// Configuration for the background job system.
/// Holds registrations mapping job args types to their implementations.
/// </summary>
public sealed class BackgroundJobOptions
{
    private readonly Dictionary<Type, BackgroundJobConfiguration> _byArgsType = [];
    private readonly Dictionary<string, BackgroundJobConfiguration> _byName = [];

    /// <summary>
    /// Master switch to enable/disable background job execution.
    /// Default: <see langword="true"/>.
    /// </summary>
    public bool IsJobExecutionEnabled { get; set; } = true;

    /// <summary>
    /// Delegate to resolve a job name from an args type.
    /// Default: <see cref="BackgroundJobNameAttribute.GetName(Type)"/>.
    /// </summary>
    public Func<Type, string> JobNameResolver { get; set; } = BackgroundJobNameAttribute.GetName;

    /// <summary>
    /// Registers a job configuration.
    /// </summary>
    public void AddJob<TJob, TArgs>()
        where TJob : class, IBackgroundJob<TArgs>
    {
        AddJob(typeof(TJob), typeof(TArgs));
    }

    /// <summary>
    /// Registers a job configuration by types.
    /// </summary>
    public void AddJob(Type jobType, Type argsType)
    {
        var name = JobNameResolver(argsType);
        var config = new BackgroundJobConfiguration(jobType, argsType, name);
        _byArgsType[argsType] = config;
        _byName[name] = config;
    }

    /// <summary>
    /// Gets the job configuration for the given args type.
    /// </summary>
    public BackgroundJobConfiguration GetJob<TArgs>() => GetJob(typeof(TArgs));

    /// <summary>
    /// Gets the job configuration for the given args type.
    /// </summary>
    public BackgroundJobConfiguration GetJob(Type argsType)
    {
        if (_byArgsType.TryGetValue(argsType, out var config))
            return config;

        throw new InvalidOperationException(
            $"No background job registered for args type: {argsType.FullName}. " +
            $"Register it using AddBackgroundJob<TJob, TArgs>().");
    }

    /// <summary>
    /// Gets the job configuration by name.
    /// </summary>
    public BackgroundJobConfiguration GetJob(string name)
    {
        if (_byName.TryGetValue(name, out var config))
            return config;

        throw new InvalidOperationException(
            $"No background job registered with name: {name}.");
    }

    /// <summary>
    /// Tries to get the job configuration for the given args type.
    /// </summary>
    public bool TryGetJob(Type argsType, out BackgroundJobConfiguration? config)
        => _byArgsType.TryGetValue(argsType, out config);

    /// <summary>
    /// Tries to get the job configuration by name.
    /// </summary>
    public bool TryGetJob(string name, out BackgroundJobConfiguration? config)
        => _byName.TryGetValue(name, out config);

    /// <summary>
    /// Gets all registered job configurations.
    /// </summary>
    public IReadOnlyList<BackgroundJobConfiguration> GetJobs()
        => _byArgsType.Values.ToImmutableList();
}

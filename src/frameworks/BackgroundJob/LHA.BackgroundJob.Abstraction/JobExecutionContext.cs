namespace LHA.BackgroundJob;

/// <summary>
/// Context passed to <see cref="IBackgroundJobExecuter"/> during job execution.
/// Contains the service provider for DI resolution, the job/args types, and the deserialized arguments.
/// </summary>
public sealed class JobExecutionContext
{
    /// <summary>Scoped service provider for resolving the job and its dependencies.</summary>
    public required IServiceProvider ServiceProvider { get; init; }

    /// <summary>The concrete job implementation type (implements <see cref="IBackgroundJob{TArgs}"/>).</summary>
    public required Type JobType { get; init; }

    /// <summary>The args type for the job.</summary>
    public required Type ArgsType { get; init; }

    /// <summary>Deserialized job arguments.</summary>
    public required object JobArgs { get; init; }

    /// <summary>Cancellation token for cooperative cancellation.</summary>
    public CancellationToken CancellationToken { get; init; }
}

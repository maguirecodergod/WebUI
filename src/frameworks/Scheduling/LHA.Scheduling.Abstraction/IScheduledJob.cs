namespace LHA.Scheduling;

/// <summary>
/// Defines a schedulable job. Implement this interface per job type
/// and register in DI. The scheduler infrastructure will invoke
/// <see cref="ExecuteAsync"/> when the job is triggered.
/// </summary>
public interface IScheduledJob
{
    /// <summary>
    /// Executes the job logic.
    /// </summary>
    /// <param name="context">Execution context with job metadata, parameters, and cancellation.</param>
    /// <returns>A result indicating success, failure, or retry.</returns>
    Task<JobResult> ExecuteAsync(JobContext context);
}

/// <summary>
/// Marker interface for strongly-typed job identification.
/// Implement <see cref="IScheduledJob"/> directly for job logic;
/// use this only when you need a distinct type token for DI resolution of multiple job types.
/// </summary>
/// <typeparam name="TParam">The job parameter type (deserialized from stored state).</typeparam>
public interface IScheduledJob<TParam> : IScheduledJob where TParam : class
{
    /// <summary>
    /// Executes the job with strongly-typed parameters.
    /// The framework deserializes the parameters and wraps them in <see cref="JobContext"/>.
    /// </summary>
    Task<JobResult> ExecuteAsync(JobContext context, TParam parameters);

    /// <summary>
    /// Default implementation delegates to the typed overload by extracting parameters from context.
    /// </summary>
    async Task<JobResult> IScheduledJob.ExecuteAsync(JobContext context)
    {
        var parameters = context.GetParameters<TParam>()
            ?? throw new InvalidOperationException(
                $"Job parameters of type {typeof(TParam).Name} are required but were null.");
        return await ExecuteAsync(context, parameters);
    }
}

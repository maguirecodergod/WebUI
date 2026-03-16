namespace LHA.BackgroundJob;

/// <summary>
/// Enqueues background jobs for asynchronous execution.
/// </summary>
public interface IBackgroundJobManager
{
    /// <summary>
    /// Enqueues a background job.
    /// </summary>
    /// <typeparam name="TArgs">The job arguments type.</typeparam>
    /// <param name="args">Job arguments (will be serialized and stored).</param>
    /// <param name="priority">Job priority.</param>
    /// <param name="delay">Optional delay before the job becomes eligible for execution.</param>
    /// <returns>A unique job identifier.</returns>
    Task<string> EnqueueAsync<TArgs>(
        TArgs args,
        BackgroundJobPriority priority = BackgroundJobPriority.Normal,
        TimeSpan? delay = null);
}

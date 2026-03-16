namespace LHA.Scheduling;

/// <summary>
/// Scheduler-agnostic interface for enqueuing and scheduling jobs.
/// Application services should depend on this interface, not on Hangfire/Quartz directly.
/// </summary>
public interface IJobScheduler
{
    /// <summary>
    /// Enqueues a job for immediate execution (fire-and-forget).
    /// </summary>
    /// <typeparam name="TJob">The job implementation type.</typeparam>
    /// <param name="parameters">Optional parameters serialized and passed to the job.</param>
    /// <param name="options">Optional execution options (tenant, priority, queue, etc.).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The unique job execution ID.</returns>
    Task<string> EnqueueAsync<TJob>(
        object? parameters = null,
        JobOptions? options = null,
        CancellationToken cancellationToken = default) where TJob : IScheduledJob;

    /// <summary>
    /// Schedules a job to execute after a specified delay.
    /// </summary>
    /// <typeparam name="TJob">The job implementation type.</typeparam>
    /// <param name="delay">Time to wait before executing the job.</param>
    /// <param name="parameters">Optional parameters serialized and passed to the job.</param>
    /// <param name="options">Optional execution options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The unique job execution ID.</returns>
    Task<string> ScheduleAsync<TJob>(
        TimeSpan delay,
        object? parameters = null,
        JobOptions? options = null,
        CancellationToken cancellationToken = default) where TJob : IScheduledJob;

    /// <summary>
    /// Schedules a job to execute at a specific UTC time.
    /// </summary>
    /// <typeparam name="TJob">The job implementation type.</typeparam>
    /// <param name="enqueueAt">The UTC time to execute the job.</param>
    /// <param name="parameters">Optional parameters serialized and passed to the job.</param>
    /// <param name="options">Optional execution options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The unique job execution ID.</returns>
    Task<string> ScheduleAsync<TJob>(
        DateTimeOffset enqueueAt,
        object? parameters = null,
        JobOptions? options = null,
        CancellationToken cancellationToken = default) where TJob : IScheduledJob;

    /// <summary>
    /// Schedules a continuation job that executes after a parent job completes.
    /// Not all schedulers support this — <see cref="NotSupportedException"/> may be thrown.
    /// </summary>
    /// <typeparam name="TJob">The continuation job type.</typeparam>
    /// <param name="parentJobId">The ID of the parent job to wait for.</param>
    /// <param name="parameters">Optional parameters.</param>
    /// <param name="options">Optional execution options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The unique job execution ID.</returns>
    Task<string> ContinueWithAsync<TJob>(
        string parentJobId,
        object? parameters = null,
        JobOptions? options = null,
        CancellationToken cancellationToken = default) where TJob : IScheduledJob;

    /// <summary>
    /// Attempts to cancel/delete a scheduled or enqueued job.
    /// Returns true if the job was successfully cancelled.
    /// </summary>
    Task<bool> CancelAsync(string jobId, CancellationToken cancellationToken = default);
}

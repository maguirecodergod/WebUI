namespace LHA.Scheduling;

/// <summary>
/// Manages recurring/cron-based job registrations.
/// CRUD operations for recurring jobs that run on a schedule.
/// </summary>
public interface IRecurringJobManager
{
    /// <summary>
    /// Creates or updates a recurring job with a cron expression.
    /// If a recurring job with the same <paramref name="recurringJobId"/> exists, it is updated.
    /// </summary>
    /// <typeparam name="TJob">The job implementation type.</typeparam>
    /// <param name="recurringJobId">
    /// Unique identifier for the recurring job registration.
    /// Convention: "service-name.job-name" (e.g. "orders.cleanup-expired").
    /// </param>
    /// <param name="cronExpression">Cron expression defining the schedule (see <see cref="CronPresets"/>).</param>
    /// <param name="options">Optional recurring job configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddOrUpdateAsync<TJob>(
        string recurringJobId,
        string cronExpression,
        RecurringJobOptions? options = null,
        CancellationToken cancellationToken = default) where TJob : IScheduledJob;

    /// <summary>
    /// Removes a recurring job registration.
    /// </summary>
    /// <param name="recurringJobId">The recurring job ID to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RemoveAsync(string recurringJobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Triggers a recurring job to execute immediately (outside its normal schedule).
    /// </summary>
    /// <param name="recurringJobId">The recurring job ID to trigger.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task TriggerAsync(string recurringJobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a recurring job with the given ID exists.
    /// </summary>
    Task<bool> ExistsAsync(string recurringJobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all registered recurring job definitions.
    /// </summary>
    Task<IReadOnlyList<RecurringJobDefinition>> ListAsync(CancellationToken cancellationToken = default);
}

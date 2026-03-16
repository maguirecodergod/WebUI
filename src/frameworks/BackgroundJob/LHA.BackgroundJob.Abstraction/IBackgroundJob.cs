namespace LHA.BackgroundJob;

/// <summary>
/// Defines a background job that processes arguments of type <typeparamref name="TArgs"/>.
/// Implement this interface per job type and register via
/// <see cref="BackgroundJobServiceCollectionExtensions.AddBackgroundJob{TJob, TArgs}"/>.
/// </summary>
/// <typeparam name="TArgs">The job arguments type (serialized and stored).</typeparam>
public interface IBackgroundJob<in TArgs>
{
    /// <summary>
    /// Executes the job with the given arguments.
    /// </summary>
    /// <param name="args">Deserialized job arguments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ExecuteAsync(TArgs args, CancellationToken cancellationToken = default);
}

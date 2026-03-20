using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LHA.BackgroundJob;

/// <summary>
/// No-op implementation of <see cref="IBackgroundJobManager"/>.
/// Registered by default; replaced when a real provider (Default, Hangfire, Quartz, RabbitMQ) is added.
/// </summary>
public sealed class NullBackgroundJobManager : IBackgroundJobManager
{
    /// <inheritdoc />
    public Task<string> EnqueueAsync<TArgs>(
        TArgs args,
        CBackgroundJobPriority priority = CBackgroundJobPriority.Normal,
        TimeSpan? delay = null)
    {
        throw new InvalidOperationException(
            "No background job provider is configured. " +
            "Register a provider using AddLHADefaultBackgroundJobs(), AddLHAHangfireBackgroundJobs(), " +
            "AddLHAQuartzBackgroundJobs(), or AddLHARabbitMqBackgroundJobs().");
    }
}

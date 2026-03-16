using Microsoft.Extensions.Logging;
using Quartz;

namespace LHA.Scheduling.Quartz;

/// <summary>
/// Health check for Quartz.NET scheduler infrastructure.
/// Verifies that the scheduler is started and reports job/trigger counts.
/// </summary>
public sealed class QuartzHealthCheck : ISchedulingHealthCheck
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly ILogger<QuartzHealthCheck> _logger;

    public string SchedulerType => "Quartz";

    public QuartzHealthCheck(
        ISchedulerFactory schedulerFactory,
        ILogger<QuartzHealthCheck> logger)
    {
        _schedulerFactory = schedulerFactory;
        _logger = logger;
    }

    public async Task<SchedulerHealthResult> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            var metadata = await scheduler.GetMetaData(cancellationToken);

            var data = new Dictionary<string, object>
            {
                ["schedulerName"] = metadata.SchedulerName,
                ["started"] = metadata.Started,
                ["schedulerInstanceId"] = metadata.SchedulerInstanceId,
                ["threadPoolSize"] = metadata.ThreadPoolSize,
                ["runningSince"] = metadata.RunningSince?.ToString("O") ?? "never",
                ["jobStoreClustered"] = metadata.JobStoreClustered,
                ["jobStoreSupportsPersistence"] = metadata.JobStoreSupportsPersistence,
                ["numberOfJobsExecuted"] = metadata.NumberOfJobsExecuted
            };

            if (!metadata.Started || metadata.InStandbyMode)
            {
                return new SchedulerHealthResult
                {
                    IsHealthy = false,
                    Description = metadata.InStandbyMode
                        ? "Quartz scheduler is in standby mode — jobs will not execute"
                        : "Quartz scheduler has not been started",
                    Data = data
                };
            }

            return new SchedulerHealthResult
            {
                IsHealthy = true,
                Description = $"Quartz healthy: {metadata.SchedulerName}, " +
                              $"pool size {metadata.ThreadPoolSize}, " +
                              $"{metadata.NumberOfJobsExecuted} jobs executed",
                Data = data
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Quartz health check failed");

            return SchedulerHealthResult.Unhealthy(
                $"Quartz health check exception: {ex.Message}", ex);
        }
    }
}

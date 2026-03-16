using Hangfire;
using Microsoft.Extensions.Logging;

namespace LHA.Scheduling.Hangfire;

/// <summary>
/// Health check for Hangfire scheduler infrastructure.
/// Verifies that storage is accessible and the server is processing jobs.
/// </summary>
public sealed class HangfireHealthCheck : ISchedulingHealthCheck
{
    private readonly ILogger<HangfireHealthCheck> _logger;

    public string SchedulerType => "Hangfire";

    public HangfireHealthCheck(ILogger<HangfireHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<SchedulerHealthResult> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var monitoringApi = JobStorage.Current.GetMonitoringApi();
            var stats = monitoringApi.GetStatistics();

            var data = new Dictionary<string, object>
            {
                ["servers"] = stats.Servers,
                ["queues"] = stats.Queues,
                ["enqueued"] = stats.Enqueued,
                ["scheduled"] = stats.Scheduled,
                ["processing"] = stats.Processing,
                ["succeeded"] = stats.Succeeded,
                ["failed"] = stats.Failed,
                ["recurring"] = stats.Recurring
            };

            if (stats.Servers == 0)
            {
                return Task.FromResult(new SchedulerHealthResult
                {
                    IsHealthy = false,
                    Description = "No Hangfire servers detected — jobs will not be processed",
                    Data = data
                });
            }

            return Task.FromResult(new SchedulerHealthResult
            {
                IsHealthy = true,
                Description = $"Hangfire healthy: {stats.Servers} server(s), " +
                              $"{stats.Enqueued} enqueued, {stats.Processing} processing",
                Data = data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hangfire health check failed");
            return Task.FromResult(SchedulerHealthResult.Unhealthy(
                "Failed to query Hangfire storage", ex));
        }
    }
}

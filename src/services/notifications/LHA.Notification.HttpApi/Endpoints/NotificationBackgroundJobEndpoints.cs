using Hangfire;
using Hangfire.Storage;
using LHA.Ddd.Application;
using LHA.Shared.Contracts.BackgroundJobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace LHA.Notification.HttpApi;

/// <summary>
/// Notification Background Job endpoints.
/// </summary>
public static class NotificationBackgroundJobEndpoints
{
    /// <summary>
    /// Maps endpoints for Notification Background Job management.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapNotificationBackgroundJobEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("Notification", "/api/v{version:apiVersion}/notification/background-jobs")
            .WithTags("Notification Background Jobs")
            .RequireAuthorization();

        // 1. Get Recurring Jobs
        group.MapGet("/recurring", ([FromServices] JobStorage jobStorage) =>
        {
            using var connection = jobStorage.GetConnection();
            var recurringJobs = connection.GetRecurringJobs();

            var result = recurringJobs.Select(job => new Shared.Contracts.BackgroundJobs.RecurringJobDto
            {
                Id = job.Id,
                Cron = job.Cron,
                Queue = job.Queue,
                NextExecution = job.NextExecution,
                LastExecution = job.LastExecution,
                LastJobState = job.LastJobState,
                LastJobId = job.LastJobId,
                TimeZoneId = job.TimeZoneId,
                CreatedAt = job.CreatedAt,
                Error = job.Error,
                RetryAttempt = job.RetryAttempt,
                Removed = job.Removed,
                JobMethod = job.Job?.Method.Name,
                LoadExceptionMessage = job.LoadException?.Message
            }).ToList();

            return Results.Ok(ApiResponse<List<Shared.Contracts.BackgroundJobs.RecurringJobDto>>.Ok(result));
        });

        // 2. Trigger a Recurring Job
        group.MapPost("/recurring/{id}/trigger", ([FromServices] IRecurringJobManager recurringJobManager, string id) =>
        {
            recurringJobManager.Trigger(id);
            return Results.Ok(ApiResponse<bool>.Ok(true));
        });

        // 3. Delete a Recurring Job
        group.MapDelete("/recurring/{id}", ([FromServices] IRecurringJobManager recurringJobManager, string id) =>
        {
            recurringJobManager.RemoveIfExists(id);
            return Results.Ok(ApiResponse<bool>.Ok(true));
        });

        // 4. Get Enqueued Jobs
        group.MapGet("/enqueued", ([FromServices] JobStorage jobStorage, string queue = "default") =>
        {
            var monitoringApi = jobStorage.GetMonitoringApi();
            var enqueuedJobs = monitoringApi.EnqueuedJobs(queue, 0, 50);

            var result = enqueuedJobs.Select(x => new EnqueuedJobDto
            {
                JobId = x.Key,
                Method = x.Value.Job?.Method.Name,
                State = x.Value.State
            }).ToList();

            return Results.Ok(ApiResponse<List<EnqueuedJobDto>>.Ok(result));
        });

        // 5. Get Failed Jobs
        group.MapGet("/failed", ([FromServices] JobStorage jobStorage) =>
        {
            var monitoringApi = jobStorage.GetMonitoringApi();
            var failedJobs = monitoringApi.FailedJobs(from: 0, count: 50);

            var result = failedJobs.Select(x => new FailedJobDto
            {
                JobId = x.Key,
                Method = x.Value.Job?.Method.Name,
                ExceptionDetails = x.Value.ExceptionDetails,
                FailedAt = x.Value.FailedAt
            }).ToList();

            return Results.Ok(ApiResponse<List<FailedJobDto>>.Ok(result));
        });

        // 6. Delete a specific Job (e.g., failed job)
        group.MapDelete("/{jobId}", ([FromServices] IBackgroundJobClient backgroundJobClient, string jobId) =>
        {
            var deleted = backgroundJobClient.Delete(jobId);
            return Results.Ok(ApiResponse<bool>.Ok(deleted));
        });

        // 7. Requeue a specific Job (e.g., retry failed job)
        group.MapPost("/{jobId}/requeue", ([FromServices] IBackgroundJobClient backgroundJobClient, string jobId) =>
        {
            var requeued = backgroundJobClient.Requeue(jobId);
            return Results.Ok(ApiResponse<bool>.Ok(requeued));
        });

        // 8. Queues
        group.MapGet("/queues", ([FromServices] JobStorage jobStorage) =>
        {
            var monitoringApi = jobStorage.GetMonitoringApi();
            var queues = monitoringApi.Queues();
            var result = queues.Select(q => new QueueWithTopEnqueuedJobsDto
            {
                Name = q.Name,
                Length = q.Length,
                Fetched = q.Fetched,
                FirstJobs = q.FirstJobs.Select(fj => new EnqueuedJobDto
                {
                    JobId = fj.Key,
                    Method = fj.Value.Job?.Method.Name,
                    State = fj.Value.State
                }).ToList()
            }).ToList();
            return Results.Ok(ApiResponse<List<QueueWithTopEnqueuedJobsDto>>.Ok(result));
        });

        // 9. Servers
        group.MapGet("/servers", ([FromServices] JobStorage jobStorage) =>
        {
            var monitoringApi = jobStorage.GetMonitoringApi();
            var servers = monitoringApi.Servers();
            var result = servers.Select(s => new ServerDto
            {
                Name = s.Name,
                WorkersCount = s.WorkersCount,
                StartedAt = s.StartedAt,
                Queues = s.Queues?.ToList() ?? new List<string>(),
                Heartbeat = s.Heartbeat
            }).ToList();
            return Results.Ok(ApiResponse<List<ServerDto>>.Ok(result));
        });

        // 10. Job Details
        group.MapGet("/{jobId}/details", ([FromServices] JobStorage jobStorage, string jobId) =>
        {
            var monitoringApi = jobStorage.GetMonitoringApi();
            var details = monitoringApi.JobDetails(jobId);
            if (details == null) return Results.NotFound(ApiResponse<object>.Fail(404, "JOB_NOT_FOUND", "Job not found"));

            var result = new JobDetailsDto
            {
                JobMethod = details.Job?.Method.Name,
                LoadExceptionMessage = details.LoadException?.Message,
                CreatedAt = details.CreatedAt,
                Properties = details.Properties?.ToDictionary(k => k.Key, v => v.Value?.ToString() ?? string.Empty) ?? new(),
                History = details.History?.Select(h => new StateHistoryDto
                {
                    StateName = h.StateName,
                    CreatedAt = h.CreatedAt,
                    Reason = h.Reason,
                    Data = h.Data?.ToDictionary(k => k.Key, v => v.Value) ?? new()
                }).ToList() ?? new(),
                ExpireAt = details.ExpireAt
            };
            return Results.Ok(ApiResponse<JobDetailsDto>.Ok(result));
        });

        // 11. Statistics
        group.MapGet("/statistics", ([FromServices] JobStorage jobStorage) =>
        {
            var monitoringApi = jobStorage.GetMonitoringApi();
            var stats = monitoringApi.GetStatistics();
            var result = new StatisticsDto
            {
                Servers = stats.Servers,
                Recurring = stats.Recurring,
                Enqueued = stats.Enqueued,
                Queues = stats.Queues,
                Scheduled = stats.Scheduled,
                Processing = stats.Processing,
                Succeeded = stats.Succeeded,
                Failed = stats.Failed,
                Deleted = stats.Deleted,
                Retries = stats.Retries,
                Awaiting = stats.Awaiting
            };
            return Results.Ok(ApiResponse<StatisticsDto>.Ok(result));
        });

        return endpoints;
    }
}

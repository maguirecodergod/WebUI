using Hangfire;
using Hangfire.Storage;
using LHA.Ddd.Application;
using LHA.Shared.Contracts.BackgroundJobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace LHA.Account.HttpApi;

/// <summary>
/// Background job management endpoints powered by Hangfire.
/// Provides monitoring, scheduling, and administration capabilities.
/// </summary>
public static class BackgroundJobEndpoints
{
    /// <summary>
    /// Maps all background job management endpoints.
    /// </summary>
    public static IEndpointRouteBuilder MapBackgroundJobEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/account/background-jobs")
            .WithTags("Background Jobs");

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
        })
        .Produces<ApiResponse<List<LHA.Shared.Contracts.BackgroundJobs.RecurringJobDto>>>();

        // 2. Trigger a Recurring Job
        group.MapPost("/recurring/{id}/trigger", ([FromServices] IRecurringJobManager recurringJobManager, string id) =>
        {
            recurringJobManager.Trigger(id);
            return Results.Ok(ApiResponse<bool>.Ok(true));
        })
        .Produces<ApiResponse<bool>>();

        // 3. Delete a Recurring Job
        group.MapDelete("/recurring/{id}", ([FromServices] IRecurringJobManager recurringJobManager, string id) =>
        {
            recurringJobManager.RemoveIfExists(id);
            return Results.Ok(ApiResponse<bool>.Ok(true));
        })
        .Produces<ApiResponse<bool>>();

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
        })
        .Produces<ApiResponse<List<EnqueuedJobDto>>>();

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
        })
        .Produces<ApiResponse<List<FailedJobDto>>>();

        // 6. Delete a specific Job (e.g., failed job)
        group.MapDelete("/{jobId}", ([FromServices] IBackgroundJobClient backgroundJobClient, string jobId) =>
        {
            var deleted = backgroundJobClient.Delete(jobId);
            return Results.Ok(ApiResponse<bool>.Ok(deleted));
        })
        .Produces<ApiResponse<bool>>();

        // 7. Requeue a specific Job (e.g., retry failed job)
        group.MapPost("/{jobId}/requeue", ([FromServices] IBackgroundJobClient backgroundJobClient, string jobId) =>
        {
            var requeued = backgroundJobClient.Requeue(jobId);
            return Results.Ok(ApiResponse<bool>.Ok(requeued));
        })
        .Produces<ApiResponse<bool>>();

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
        })
        .Produces<ApiResponse<List<QueueWithTopEnqueuedJobsDto>>>();

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
        })
        .Produces<ApiResponse<List<ServerDto>>>();

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
        })
        .Produces<ApiResponse<JobDetailsDto>>();

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
        })
        .Produces<ApiResponse<StatisticsDto>>();

        // 12. Fetched Jobs
        group.MapGet("/fetched", ([FromServices] JobStorage jobStorage, string queue = "default", int from = 0, int perPage = 50) =>
        {
            var monitoringApi = jobStorage.GetMonitoringApi();
            var jobs = monitoringApi.FetchedJobs(queue, from, perPage);
            var result = jobs.Select(x => new FetchedJobDto
            {
                JobId = x.Key,
                JobMethod = x.Value.Job?.Method.Name,
                LoadExceptionMessage = x.Value.LoadException?.Message,
                State = x.Value.State,
                FetchedAt = x.Value.FetchedAt
            }).ToList();
            return Results.Ok(ApiResponse<List<FetchedJobDto>>.Ok(result));
        })
        .Produces<ApiResponse<List<FetchedJobDto>>>();

        // 13. Processing Jobs
        group.MapGet("/processing", ([FromServices] JobStorage jobStorage, int from = 0, int count = 50) =>
        {
            var monitoringApi = jobStorage.GetMonitoringApi();
            var jobs = monitoringApi.ProcessingJobs(from, count);
            var result = jobs.Select(x => new ProcessingJobDto
            {
                JobId = x.Key,
                JobMethod = x.Value.Job?.Method.Name,
                LoadExceptionMessage = x.Value.LoadException?.Message,
                InProcessingState = x.Value.InProcessingState,
                ServerId = x.Value.ServerId,
                StartedAt = x.Value.StartedAt
            }).ToList();
            return Results.Ok(ApiResponse<List<ProcessingJobDto>>.Ok(result));
        })
        .Produces<ApiResponse<List<ProcessingJobDto>>>();

        // 14. Scheduled Jobs
        group.MapGet("/scheduled", ([FromServices] JobStorage jobStorage, int from = 0, int count = 50) =>
        {
            var monitoringApi = jobStorage.GetMonitoringApi();
            var jobs = monitoringApi.ScheduledJobs(from, count);
            var result = jobs.Select(x => new ScheduledJobDto
            {
                JobId = x.Key,
                JobMethod = x.Value.Job?.Method.Name,
                LoadExceptionMessage = x.Value.LoadException?.Message,
                EnqueueAt = x.Value.EnqueueAt,
                ScheduledAt = x.Value.ScheduledAt,
                InScheduledState = x.Value.InScheduledState
            }).ToList();
            return Results.Ok(ApiResponse<List<ScheduledJobDto>>.Ok(result));
        })
        .Produces<ApiResponse<List<ScheduledJobDto>>>();

        // 15. Succeeded Jobs
        group.MapGet("/succeeded", ([FromServices] JobStorage jobStorage, int from = 0, int count = 50) =>
        {
            var monitoringApi = jobStorage.GetMonitoringApi();
            var jobs = monitoringApi.SucceededJobs(from, count);
            var result = jobs.Select(x => new SucceededJobDto
            {
                JobId = x.Key,
                JobMethod = x.Value.Job?.Method.Name,
                LoadExceptionMessage = x.Value.LoadException?.Message,
                Result = x.Value.Result?.ToString(),
                TotalDuration = x.Value.TotalDuration,
                SucceededAt = x.Value.SucceededAt,
                InSucceededState = x.Value.InSucceededState
            }).ToList();
            return Results.Ok(ApiResponse<List<SucceededJobDto>>.Ok(result));
        })
        .Produces<ApiResponse<List<SucceededJobDto>>>();

        // 16. Deleted Jobs
        group.MapGet("/deleted", ([FromServices] JobStorage jobStorage, int from = 0, int count = 50) =>
        {
            var monitoringApi = jobStorage.GetMonitoringApi();
            var jobs = monitoringApi.DeletedJobs(from, count);
            var result = jobs.Select(x => new DeletedJobDto
            {
                JobId = x.Key,
                JobMethod = x.Value.Job?.Method.Name,
                LoadExceptionMessage = x.Value.LoadException?.Message,
                DeletedAt = x.Value.DeletedAt,
                InDeletedState = x.Value.InDeletedState
            }).ToList();
            return Results.Ok(ApiResponse<List<DeletedJobDto>>.Ok(result));
        })
        .Produces<ApiResponse<List<DeletedJobDto>>>();

        // 17. Succeeded by Dates
        group.MapGet("/succeeded-by-dates", ([FromServices] JobStorage jobStorage) =>
        {
            var result = jobStorage.GetMonitoringApi().SucceededByDatesCount();
            return Results.Ok(ApiResponse<Dictionary<DateTime, long>>.Ok(new Dictionary<DateTime, long>(result)));
        })
        .Produces<ApiResponse<Dictionary<DateTime, long>>>();

        // 18. Failed by Dates
        group.MapGet("/failed-by-dates", ([FromServices] JobStorage jobStorage) =>
        {
            var result = jobStorage.GetMonitoringApi().FailedByDatesCount();
            return Results.Ok(ApiResponse<Dictionary<DateTime, long>>.Ok(new Dictionary<DateTime, long>(result)));
        })
        .Produces<ApiResponse<Dictionary<DateTime, long>>>();

        // 19. Hourly Succeeded
        group.MapGet("/hourly-succeeded", ([FromServices] JobStorage jobStorage) =>
        {
            var result = jobStorage.GetMonitoringApi().HourlySucceededJobs();
            return Results.Ok(ApiResponse<Dictionary<DateTime, long>>.Ok(new Dictionary<DateTime, long>(result)));
        })
        .Produces<ApiResponse<Dictionary<DateTime, long>>>();

        // 20. Hourly Failed
        group.MapGet("/hourly-failed", ([FromServices] JobStorage jobStorage) =>
        {
            var result = jobStorage.GetMonitoringApi().HourlyFailedJobs();
            return Results.Ok(ApiResponse<Dictionary<DateTime, long>>.Ok(new Dictionary<DateTime, long>(result)));
        })
        .Produces<ApiResponse<Dictionary<DateTime, long>>>();

        return endpoints;
    }
}

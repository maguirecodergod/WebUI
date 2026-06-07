using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.Shared.Contracts.BackgroundJobs;

namespace LHA.BlazorWasm.HttpApi.Client.Clients.BackgroundJobs;

/// <summary>
/// Represents the API client for background jobs.
/// </summary>
public class BackgroundJobApiClient : ApiClientBase
{
    private const string BaseUrl = "api/v1/account/background-jobs";

    /// <summary>
    /// Initializes a new instance of the <see cref="BackgroundJobApiClient"/> class.
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="errorHandler"></param>
    public BackgroundJobApiClient(HttpClient httpClient, IApiErrorHandler errorHandler)
        : base(httpClient, errorHandler)
    {
    }

    /// <summary>
    /// Gets the list of recurring jobs.
    /// </summary>
    /// <returns></returns>
    public async Task<List<RecurringJobDto>?> GetRecurringJobsAsync()
    {
        var response = await GetAsync<List<RecurringJobDto>>($"{BaseUrl}/recurring");
        return response.Result.Data;
    }

    /// <summary>
    /// Triggers a recurring job by ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> TriggerRecurringJobAsync(string id)
    {
        var response = await PostAsync<object, bool>($"{BaseUrl}/recurring/{id}/trigger", new { });
        return response.Result.Data;
    }

    /// <summary>
    /// Deletes a recurring job by ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> DeleteRecurringJobAsync(string id)
    {
        var response = await DeleteAsync<bool>($"{BaseUrl}/recurring/{id}");
        return response.Result.Data;
    }

    /// <summary>
    /// Gets the list of enqueued jobs.
    /// </summary>
    /// <param name="queue"></param>
    /// <returns></returns>
    public async Task<List<EnqueuedJobDto>?> GetEnqueuedJobsAsync(string queue = "default")
    {
        var response = await GetAsync<List<EnqueuedJobDto>>($"{BaseUrl}/enqueued?queue={queue}");
        return response.Result.Data;
    }

    /// <summary>
    /// Gets the list of failed jobs.
    /// </summary>
    /// <returns></returns>
    public async Task<List<FailedJobDto>?> GetFailedJobsAsync()
    {
        var response = await GetAsync<List<FailedJobDto>>($"{BaseUrl}/failed");
        return response.Result.Data;
    }

    /// <summary>
    /// Deletes a job by ID.
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public async Task<bool> DeleteJobAsync(string jobId)
    {
        var response = await DeleteAsync<bool>($"{BaseUrl}/{jobId}");
        return response.Result.Data;
    }

    /// <summary>
    /// Requeues a job by ID.
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public async Task<bool> RequeueJobAsync(string jobId)
    {
        var response = await PostAsync<object, bool>($"{BaseUrl}/{jobId}/requeue", new { });
        return response.Result.Data;
    }

    /// <summary>
    /// Gets the list of queues.
    /// </summary>
    /// <returns></returns>
    public async Task<List<QueueWithTopEnqueuedJobsDto>?> GetQueuesAsync()
    {
        var response = await GetAsync<List<QueueWithTopEnqueuedJobsDto>>($"{BaseUrl}/queues");
        return response.Result.Data;
    }

    /// <summary>
    /// Gets the list of servers.
    /// </summary>
    /// <returns></returns>
    public async Task<List<ServerDto>?> GetServersAsync()
    {
        var response = await GetAsync<List<ServerDto>>($"{BaseUrl}/servers");
        return response.Result.Data;
    }

    /// <summary>
    /// Gets the details of a job by ID.
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public async Task<JobDetailsDto?> GetJobDetailsAsync(string jobId)
    {
        var response = await GetAsync<JobDetailsDto>($"{BaseUrl}/{jobId}/details");
        return response.Result.Data;
    }

    /// <summary>
    /// Gets the statistics of background jobs.
    /// </summary>
    /// <returns></returns>
    public async Task<StatisticsDto?> GetStatisticsAsync()
    {
        var response = await GetAsync<StatisticsDto>($"{BaseUrl}/statistics");
        return response.Result.Data;
    }

    /// <summary>
    /// Gets the list of fetched jobs.
    /// </summary>
    /// <param name="queue"></param>
    /// <param name="from"></param>
    /// <param name="perPage"></param>
    /// <returns></returns>
    public async Task<List<FetchedJobDto>?> GetFetchedJobsAsync(string queue = "default", int from = 0, int perPage = 50)
    {
        var response = await GetAsync<List<FetchedJobDto>>($"{BaseUrl}/fetched?queue={queue}&from={from}&perPage={perPage}");
        return response.Result.Data;
    }

    /// <summary>
    /// Gets the list of processing jobs.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task<List<ProcessingJobDto>?> GetProcessingJobsAsync(int from = 0, int count = 50)
    {
        var response = await GetAsync<List<ProcessingJobDto>>($"{BaseUrl}/processing?from={from}&count={count}");
        return response.Result.Data;
    }

    /// <summary>
    /// Gets the list of scheduled jobs.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task<List<ScheduledJobDto>?> GetScheduledJobsAsync(int from = 0, int count = 50)
    {
        var response = await GetAsync<List<ScheduledJobDto>>($"{BaseUrl}/scheduled?from={from}&count={count}");
        return response.Result.Data;
    }

    /// <summary>
    /// Gets the list of succeeded jobs.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task<List<SucceededJobDto>?> GetSucceededJobsAsync(int from = 0, int count = 50)
    {
        var response = await GetAsync<List<SucceededJobDto>>($"{BaseUrl}/succeeded?from={from}&count={count}");
        return response.Result.Data;
    }

    /// <summary>
    /// Gets the list of deleted jobs.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task<List<DeletedJobDto>?> GetDeletedJobsAsync(int from = 0, int count = 50)
    {
        var response = await GetAsync<List<DeletedJobDto>>($"{BaseUrl}/deleted?from={from}&count={count}");
        return response.Result.Data;
    }

    /// <summary>
    /// Gets the succeeded jobs by dates.
    /// </summary>
    /// <returns></returns>
    public async Task<Dictionary<DateTime, long>?> GetSucceededByDatesAsync()
    {
        var response = await GetAsync<Dictionary<DateTime, long>>($"{BaseUrl}/succeeded-by-dates");
        return response.Result.Data;
    }

    /// <summary>
    /// Gets the failed jobs by dates.
    /// </summary>
    /// <returns></returns>
    public async Task<Dictionary<DateTime, long>?> GetFailedByDatesAsync()
    {
        var response = await GetAsync<Dictionary<DateTime, long>>($"{BaseUrl}/failed-by-dates");
        return response.Result.Data;
    }

    /// <summary>
    /// Gets the hourly jobs by dates.
    /// </summary>
    /// <returns></returns>
    public async Task<Dictionary<DateTime, long>?> GetHourlySucceededAsync()
    {
        var response = await GetAsync<Dictionary<DateTime, long>>($"{BaseUrl}/hourly-succeeded");
        return response.Result.Data;
    }

    /// <summary>
    /// Gets the hourly failed jobs by dates.
    /// </summary>
    /// <returns></returns>
    public async Task<Dictionary<DateTime, long>?> GetHourlyFailedAsync()
    {
        var response = await GetAsync<Dictionary<DateTime, long>>($"{BaseUrl}/hourly-failed");
        return response.Result.Data;
    }
}

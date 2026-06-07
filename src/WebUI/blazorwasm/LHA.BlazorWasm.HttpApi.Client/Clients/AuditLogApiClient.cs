using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.Ddd.Application;
using LHA.Shared.Contracts.AuditLog;
using LHA.Shared.Domain.AuditLogActions;
using LHA.Shared.Domain.AuditLogs;
using LHA.Shared.Domain.EntityChanges;
using LHA.Shared.Domain.EntityPropertyChanges;
using Microsoft.AspNetCore.WebUtilities;

namespace LHA.BlazorWasm.HttpApi.Client.Clients;

/// <summary>
/// Audit Log API client.
/// </summary>
public class AuditLogApiClient : ApiClientBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuditLogApiClient"/> class.
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="errorHandler"></param>
    public AuditLogApiClient(HttpClient httpClient, IApiErrorHandler errorHandler)
        : base(httpClient, errorHandler)
    {
    }

    private string GetBaseUrl(CServiceType service) => service switch
    {
        CServiceType.Notification => "api/v1/notification/audit-logs",
        CServiceType.Mega => "api/v1/mega/audit-logs",
        CServiceType.Movie => "api/v1/movie/audit-logs",
        _ => "api/v1/account/audit-logs"
    };

    /// <summary>
    /// Gets the list of audit logs.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<AuditLogDto>> GetListAsync(AuditLogPagedRequest input, CServiceType service = CServiceType.Account)
    {
        var url = BuildQueryString(GetBaseUrl(service), input);
        var response = await GetAsync<PagedResultDto<AuditLogDto>>(url);
        return response.Result.Data!;
    }

    /// <summary>
    /// Gets the audit log by ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<AuditLogDto> GetAsync(Guid id, CServiceType service = CServiceType.Account)
    {
        var response = await GetAsync<AuditLogDto>($"{GetBaseUrl(service)}/{id}");
        return response.Result.Data!;
    }

    /// <summary>
    /// Gets the list of audit log actions.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<AuditLogActionDto>> GetActionsAsync(AuditLogActionPagedRequest input, CServiceType service = CServiceType.Account)
    {
        var url = BuildQueryString($"{GetBaseUrl(service)}/actions", input);
        var response = await GetAsync<PagedResultDto<AuditLogActionDto>>(url);
        return response.Result.Data!;
    }

    /// <summary>
    /// Gets the list of entity changes.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<EntityChangeDto>> GetEntityChangesAsync(EntityChangePagedRequest input, CServiceType service = CServiceType.Account)
    {
        var url = BuildQueryString($"{GetBaseUrl(service)}/entity-changes", input);
        var response = await GetAsync<PagedResultDto<EntityChangeDto>>(url);
        return response.Result.Data!;
    }

    /// <summary>
    /// Gets the list of entity property changes.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<EntityPropertyChangeDto>> GetEntityPropertyChangesAsync(EntityPropertyChangePagedRequest input,
        CServiceType service = CServiceType.Account)
    {
        var url = BuildQueryString($"{GetBaseUrl(service)}/entity-property-changes", input);
        var response = await GetAsync<PagedResultDto<EntityPropertyChangeDto>>(url);
        return response.Result.Data!;
    }

    /// <summary>
    /// Deletes the audit log by ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task DeleteAsync(Guid id, CServiceType service = CServiceType.Account)
    {
        await DeleteAsync<object>($"{GetBaseUrl(service)}/{id}");
    }

    /// <summary>
    /// Deletes all audit logs older than the specified cutoff.
    /// </summary>
    /// <param name="cutoffTime"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<int> DeleteOlderThanAsync(DateTimeOffset cutoffTime, CServiceType service = CServiceType.Account)
    {
        var url = QueryHelpers.AddQueryString($"{GetBaseUrl(service)}/older-than", "cutoffTime", cutoffTime.ToString("o"));
        var response = await DeleteAsync<int>(url);
        return response.Result.Data;
    }

    private string BuildQueryString<T>(string baseUrl, T input) where T : class
    {
        if (input == null) return baseUrl;

        var queryParams = new List<KeyValuePair<string, string?>>();
        AddQueryParameters(queryParams, input);

        return QueryHelpers.AddQueryString(baseUrl, queryParams);
    }

    private static void AddQueryParameters(
        List<KeyValuePair<string, string?>> queryParams,
        object input)
    {
        foreach (var prop in input.GetType().GetProperties())
        {
            if (prop.Name == "Sorter")
            {
                continue;
            }

            var val = prop.GetValue(input);
            if (val == null)
            {
                continue;
            }

            if (val is string[] strings)
            {
                foreach (var item in strings.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    queryParams.Add(new KeyValuePair<string, string?>(prop.Name, item));
                }

                continue;
            }

            if (val is System.Collections.IEnumerable values && val is not string)
            {
                foreach (var item in values)
                {
                    AddValue(queryParams, prop.Name, item);
                }

                continue;
            }

            if (prop.Name == "Filter")
            {
                AddQueryParameters(queryParams, val);
                continue;
            }

            AddValue(queryParams, prop.Name, val);
        }
    }

    private static void AddValue(
        List<KeyValuePair<string, string?>> queryParams,
        string name,
        object? value)
    {
        if (value == null)
        {
            return;
        }

        var formatted = value switch
        {
            DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("o"),
            DateTime dateTime => dateTime.ToString("o"),
            bool boolean => boolean.ToString().ToLowerInvariant(),
            Guid guid => guid.ToString(),
            Enum enumValue => enumValue.ToString(),
            _ => value.ToString()
        };

        if (!string.IsNullOrWhiteSpace(formatted))
        {
            queryParams.Add(new KeyValuePair<string, string?>(name, formatted));
        }
    }
}

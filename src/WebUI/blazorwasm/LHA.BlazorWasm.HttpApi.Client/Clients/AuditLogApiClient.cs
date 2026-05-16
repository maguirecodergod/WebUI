using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.Ddd.Application;
using LHA.Shared.Contracts.AuditLog;
using Microsoft.AspNetCore.WebUtilities;

namespace LHA.BlazorWasm.HttpApi.Client.Clients;

public class AuditLogApiClient : ApiClientBase, IAuditLogAppService
{
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

    public async Task<PagedResultDto<AuditLogDto>> GetListAsync(GetAuditLogsInput input, CServiceType service = CServiceType.Account)
    {
        var url = BuildQueryString(GetBaseUrl(service), input);
        var response = await GetAsync<PagedResultDto<AuditLogDto>>(url);
        return response.Result.Data!;
    }

    public async Task<PagedResultDto<AuditLogDto>> GetHostListAsync(GetAuditLogsInput input, CServiceType service = CServiceType.Account)
    {
        var url = BuildQueryString(GetBaseUrl(service), input);
        var response = await GetAsync<PagedResultDto<AuditLogDto>>(url);
        return response.Result.Data!;
    }

    public async Task<AuditLogDto> GetAsync(Guid id, CServiceType service = CServiceType.Account)
    {
        var response = await GetAsync<AuditLogDto>($"{GetBaseUrl(service)}/{id}");
        return response.Result.Data!;
    }

    public async Task<PagedResultDto<AuditLogActionDto>> GetActionsAsync(GetAuditLogActionsInput input, CServiceType service = CServiceType.Account)
    {
        var url = BuildQueryString($"{GetBaseUrl(service)}/actions", input);
        var response = await GetAsync<PagedResultDto<AuditLogActionDto>>(url);
        return response.Result.Data!;
    }

    public async Task<PagedResultDto<EntityChangeDto>> GetEntityChangesAsync(GetEntityChangesInput input, CServiceType service = CServiceType.Account)
    {
        var url = BuildQueryString($"{GetBaseUrl(service)}/entity-changes", input);
        var response = await GetAsync<PagedResultDto<EntityChangeDto>>(url);
        return response.Result.Data!;
    }

    public async Task<PagedResultDto<EntityPropertyChangeDto>> GetEntityPropertyChangesAsync(GetEntityPropertyChangesInput input, CServiceType service = CServiceType.Account)
    {
        var url = BuildQueryString($"{GetBaseUrl(service)}/entity-property-changes", input);
        var response = await GetAsync<PagedResultDto<EntityPropertyChangeDto>>(url);
        return response.Result.Data!;
    }

    public async Task DeleteAsync(Guid id, CServiceType service = CServiceType.Account)
    {
        await DeleteAsync<object>($"{GetBaseUrl(service)}/{id}");
    }

    public async Task<int> DeleteOlderThanAsync(DateTimeOffset cutoffTime, CServiceType service = CServiceType.Account)
    {
        var url = QueryHelpers.AddQueryString($"{GetBaseUrl(service)}/older-than", "cutoffTime", cutoffTime.ToString("o"));
        var response = await DeleteAsync<int>(url);
        return response.Result.Data;
    }

    private string BuildQueryString<T>(string baseUrl, T input) where T : class
    {
        if (input == null) return baseUrl;

        var queryParams = new Dictionary<string, string?>();
        var properties = typeof(T).GetProperties();

        foreach (var prop in properties)
        {
            var val = prop.GetValue(input);
            if (val != null)
            {
                if (val is DateTimeOffset dto)
                {
                    queryParams.Add(prop.Name, dto.ToString("o"));
                }
                else if (val is bool b)
                {
                    queryParams.Add(prop.Name, b.ToString().ToLower());
                }
                else if (val is Guid g)
                {
                    queryParams.Add(prop.Name, g.ToString());
                }
                else
                {
                    queryParams.Add(prop.Name, val.ToString());
                }
            }
        }

        return QueryHelpers.AddQueryString(baseUrl, queryParams);
    }
}

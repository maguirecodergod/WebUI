using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.Ddd.Application;
using LHA.Shared.Contracts.AuditLog;
using Microsoft.AspNetCore.WebUtilities;

namespace LHA.BlazorWasm.HttpApi.Client.Clients;

public class AuditLogApiClient : ApiClientBase, IAuditLogAppService
{
    private const string BaseUrl = "api/v1/account/audit-logs";

    public AuditLogApiClient(HttpClient httpClient, IApiErrorHandler errorHandler)
        : base(httpClient, errorHandler)
    {
    }

    public async Task<PagedResultDto<AuditLogDto>> GetListAsync(GetAuditLogsInput input)
    {
        var url = BuildQueryString(BaseUrl, input);
        var response = await GetAsync<PagedResultDto<AuditLogDto>>(url);
        return response.Result.Data!;
    }

    public async Task<PagedResultDto<AuditLogDto>> GetHostListAsync(GetAuditLogsInput input)
    {
        var url = BuildQueryString($"{BaseUrl}/host", input);
        var response = await GetAsync<PagedResultDto<AuditLogDto>>(url);
        return response.Result.Data!;
    }

    public async Task<AuditLogDto> GetAsync(Guid id)
    {
        var response = await GetAsync<AuditLogDto>($"{BaseUrl}/{id}");
        return response.Result.Data!;
    }

    public async Task<PagedResultDto<AuditLogActionDto>> GetActionsAsync(GetAuditLogActionsInput input)
    {
        var url = BuildQueryString($"{BaseUrl}/actions", input);
        var response = await GetAsync<PagedResultDto<AuditLogActionDto>>(url);
        return response.Result.Data!;
    }

    public async Task<PagedResultDto<EntityChangeDto>> GetEntityChangesAsync(GetEntityChangesInput input)
    {
        var url = BuildQueryString($"{BaseUrl}/entity-changes", input);
        var response = await GetAsync<PagedResultDto<EntityChangeDto>>(url);
        return response.Result.Data!;
    }

    public async Task<PagedResultDto<EntityPropertyChangeDto>> GetEntityPropertyChangesAsync(GetEntityPropertyChangesInput input)
    {
        var url = BuildQueryString($"{BaseUrl}/entity-property-changes", input);
        var response = await GetAsync<PagedResultDto<EntityPropertyChangeDto>>(url);
        return response.Result.Data!;
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteAsync<object>($"{BaseUrl}/{id}");
    }

    public async Task<int> DeleteOlderThanAsync(DateTimeOffset cutoffTime)
    {
        var url = QueryHelpers.AddQueryString($"{BaseUrl}/older-than", "cutoffTime", cutoffTime.ToString("o"));
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
                if (val is DateTimeOffset dto) {
                     queryParams.Add(prop.Name, dto.ToString("o"));
                } else if (val is bool b) {
                     queryParams.Add(prop.Name, b.ToString().ToLower());
                } else if (val is Guid g) {
                     queryParams.Add(prop.Name, g.ToString());
                } else {
                     queryParams.Add(prop.Name, val.ToString());
                }
            }
        }
        
        return QueryHelpers.AddQueryString(baseUrl, queryParams);
    }
}

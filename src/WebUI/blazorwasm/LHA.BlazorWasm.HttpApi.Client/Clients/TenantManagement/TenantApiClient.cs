using LHA.Shared.Contracts;
using LHA.Shared.Contracts.TenantManagement;
using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.Ddd.Application;
using Microsoft.AspNetCore.WebUtilities;

namespace LHA.BlazorWasm.HttpApi.Client.Clients.TenantManagement;

public class TenantApiClient : ApiClientBase
{
    private const string BaseUrl = "api/v1/tenant-management";

    public TenantApiClient(HttpClient httpClient, IApiErrorHandler errorHandler)
        : base(httpClient, errorHandler)
    {
    }

    public async Task<PagedResultDto<TenantDto>> GetListAsync(GetTenantsInput input)
    {
        var url = BuildQueryString(BaseUrl, input);
        var result = await GetAsync<PagedResultDto<TenantDto>>(url);
        return result.Result.Data ?? new PagedResultDto<TenantDto>();
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
                if (val is bool b)
                {
                    queryParams.Add(prop.Name, b.ToString().ToLower());
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

using LHA.Shared.Contracts.TenantManagement;
using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.Ddd.Application;
using Microsoft.AspNetCore.WebUtilities;

namespace LHA.BlazorWasm.HttpApi.Client.Clients.TenantManagement;

/// <summary>
/// Represents the API client for tenant operations.
/// </summary>
public class TenantApiClient : ApiClientBase
{
    private const string BaseUrl = "api/v1/tenant-management";

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantApiClient"/> class.
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="errorHandler"></param>
    public TenantApiClient(HttpClient httpClient, IApiErrorHandler errorHandler)
        : base(httpClient, errorHandler)
    {
    }

    /// <summary>
    /// Gets a list of tenants.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
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

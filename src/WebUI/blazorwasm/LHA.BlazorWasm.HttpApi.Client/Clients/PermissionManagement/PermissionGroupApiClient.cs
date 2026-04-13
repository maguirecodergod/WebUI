using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.Ddd.Application;
using LHA.Shared.Contracts.PermissionManagement;
using Microsoft.AspNetCore.WebUtilities;

namespace LHA.BlazorWasm.HttpApi.Client.Clients.PermissionManagement;

public class PermissionGroupApiClient : ApiClientBase
{
    private const string BaseUrl = "api/v1/permission-management/groups";

    public PermissionGroupApiClient(HttpClient httpClient, IApiErrorHandler errorHandler)
        : base(httpClient, errorHandler)
    {
    }

    public async Task<PagedResultDto<PermissionGroupDto>?> GetListAsync(GetPermissionGroupsInput input)
    {
        var queryString = new Dictionary<string, string?>();
        queryString["PageNumber"] = input.PageNumber.ToString();
        queryString["PageSize"] = input.PageSize.ToString();
        if (!string.IsNullOrEmpty(input.SorterKey)) queryString["SorterKey"] = input.SorterKey;
        if (input.SorterIsAsc.HasValue) queryString["SorterIsAsc"] = input.SorterIsAsc.Value.ToString();
        if (!string.IsNullOrEmpty(input.Filter)) queryString["Filter"] = input.Filter;
        if (!string.IsNullOrEmpty(input.ServiceName)) queryString["ServiceName"] = input.ServiceName;

        var url = QueryHelpers.AddQueryString(BaseUrl, queryString);
        var response = await GetAsync<PagedResultDto<PermissionGroupDto>>(url);
        return response.Result.Data;
    }
}

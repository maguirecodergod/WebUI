using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.Ddd.Application;
using LHA.Shared.Contracts.Identity;
using LHA.Shared.Contracts.Identity.Roles;
using Microsoft.AspNetCore.WebUtilities;

namespace LHA.BlazorWasm.HttpApi.Client.Clients;

public class RoleApiClient : ApiClientBase
{
    private const string BaseUrl = "api/v1/identity/roles";

    public RoleApiClient(HttpClient httpClient, IApiErrorHandler errorHandler)
        : base(httpClient, errorHandler)
    {
    }

    public async Task<PagedResultDto<IdentityRoleDto>?> GetListAsync(GetIdentityRolesInput input)
    {
        var queryString = new Dictionary<string, string?>();
        queryString["PageNumber"] = input.PageNumber.ToString();
        queryString["PageSize"] = input.PageSize.ToString();
        if (!string.IsNullOrEmpty(input.SorterKey)) queryString["SorterKey"] = input.SorterKey;
        if (input.SorterIsAsc.HasValue) queryString["SorterIsAsc"] = input.SorterIsAsc.Value.ToString();
        if (!string.IsNullOrEmpty(input.Filter)) queryString["Filter"] = input.Filter;
        if (input.Status.HasValue) queryString["Status"] = ((int)input.Status.Value).ToString();

        var url = QueryHelpers.AddQueryString(BaseUrl, queryString);
        var response = await GetAsync<PagedResultDto<IdentityRoleDto>>(url);
        return response.Result.Data;
    }

    public async Task<List<IdentityRoleDto>?> GetAllAsync()
    {
        var response = await GetAsync<List<IdentityRoleDto>>($"{BaseUrl}/all");
        return response.Result.Data;
    }

    public async Task<IdentityRoleDto?> GetAsync(Guid id)
    {
        var response = await GetAsync<IdentityRoleDto>($"{BaseUrl}/{id}");
        return response.Result.Data;
    }

    public async Task<IdentityRoleDto?> CreateAsync(CreateIdentityRoleInput input)
    {
        var response = await PostAsync<CreateIdentityRoleInput, IdentityRoleDto>(BaseUrl, input);
        return response.Result.Data;
    }

    public async Task<IdentityRoleDto?> UpdateAsync(Guid id, UpdateIdentityRoleInput input)
    {
        var response = await PutAsync<UpdateIdentityRoleInput, IdentityRoleDto>($"{BaseUrl}/{id}", input);
        return response.Result.Data;
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteAsync<object>($"{BaseUrl}/{id}");
    }
}

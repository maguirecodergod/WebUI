using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.Ddd.Application;
using LHA.Shared.Contracts.Identity;
using LHA.Shared.Contracts.Identity.Users;
using Microsoft.AspNetCore.WebUtilities;

namespace LHA.BlazorWasm.HttpApi.Client.Clients;

/// <summary>
/// Represents the API client for user operations.
/// </summary>
public class UserApiClient : ApiClientBase
{
    private const string BaseUrl = "api/v1/identity/users";

    /// <summary>
    /// Initializes a new instance of the <see cref="UserApiClient"/> class.
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="errorHandler"></param>
    public UserApiClient(HttpClient httpClient, IApiErrorHandler errorHandler)
        : base(httpClient, errorHandler)
    {
    }

    /// <summary>
    /// Gets a list of users.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<IdentityUserDto>?> GetListAsync(GetIdentityUsersInput input)
    {
        var queryString = new Dictionary<string, string?>();
        queryString["PageNumber"] = input.PageNumber.ToString();
        queryString["PageSize"] = input.PageSize.ToString();
        if (!string.IsNullOrEmpty(input.SorterKey)) queryString["SorterKey"] = input.SorterKey;
        if (input.SorterIsAsc.HasValue) queryString["SorterIsAsc"] = input.SorterIsAsc.Value.ToString();
        if (!string.IsNullOrEmpty(input.Filter)) queryString["Filter"] = input.Filter;
        if (input.RoleId.HasValue) queryString["RoleId"] = input.RoleId.Value.ToString();
        if (input.Status.HasValue) queryString["Status"] = ((int)input.Status.Value).ToString();

        var url = QueryHelpers.AddQueryString(BaseUrl, queryString);
        var response = await GetAsync<PagedResultDto<IdentityUserDto>>(url);
        return response.Result.Data;
    }

    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IdentityUserDto?> GetAsync(Guid id)
    {
        var response = await GetAsync<IdentityUserDto>($"{BaseUrl}/{id}");
        return response.Result.Data;
    }

    /// <summary>
    /// Creates a user.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<IdentityUserDto?> CreateAsync(CreateIdentityUserInput input)
    {
        var response = await PostAsync<CreateIdentityUserInput, IdentityUserDto>(BaseUrl, input);
        return response.Result.Data;
    }

    /// <summary>
    /// Updates a user.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<IdentityUserDto?> UpdateAsync(Guid id, UpdateIdentityUserInput input)
    {
        var response = await PutAsync<UpdateIdentityUserInput, IdentityUserDto>($"{BaseUrl}/{id}", input);
        return response.Result.Data;
    }

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task DeleteAsync(Guid id)
    {
        await DeleteAsync<object>($"{BaseUrl}/{id}");
    }
    
    /// <summary>
    /// Gets the roles for a user.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<List<IdentityRoleDto>?> GetRolesAsync(Guid id)
    {
        var response = await GetAsync<List<IdentityRoleDto>>($"{BaseUrl}/{id}/roles");
        return response.Result.Data;
    }

    /// <summary>
    /// Updates the roles for a user.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="roleIds"></param>
    /// <returns></returns>
    public async Task<IdentityUserDto?> UpdateRolesAsync(Guid id, List<Guid> roleIds)
    {
        var response = await PutAsync<List<Guid>, IdentityUserDto>($"{BaseUrl}/{id}/roles", roleIds);
        return response.Result.Data;
    }
}

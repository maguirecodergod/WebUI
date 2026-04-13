using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.Shared.Contracts.Identity;
using LHA.Shared.Contracts.Identity.Permissions;
using Microsoft.AspNetCore.WebUtilities;

namespace LHA.BlazorWasm.HttpApi.Client.Clients;

public class PermissionApiClient : ApiClientBase
{
    private const string BaseUrl = "api/v1/identity/permissions";

    public PermissionApiClient(HttpClient httpClient, IApiErrorHandler errorHandler)
        : base(httpClient, errorHandler)
    {
    }

    public async Task<List<PermissionGrantDto>?> GetAsync(GetPermissionListInput input)
    {
        var queryString = new Dictionary<string, string?>
        {
            ["ProviderName"] = input.ProviderName,
            ["ProviderKey"] = input.ProviderKey
        };

        var url = QueryHelpers.AddQueryString(BaseUrl, queryString);
        var response = await GetAsync<List<PermissionGrantDto>>(url);
        return response.Result.Data;
    }

    public async Task UpdateAsync(UpdatePermissionsInput input)
    {
        await PutAsync<UpdatePermissionsInput, object>(BaseUrl, input);
    }
}

using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.BlazorWasm.Shared.Constants;
using LHA.Ddd.Application;
using LHA.Shared.Contracts.Identity;
using LHA.Shared.Contracts.Identity.Permissions;
using LHA.Shared.Contracts.PermissionManagement;
using Microsoft.AspNetCore.WebUtilities;

// Resolve ambiguity for PermissionGrantDto
using IdentityPermissionGrantDto = LHA.Shared.Contracts.Identity.PermissionGrantDto;

namespace LHA.BlazorWasm.HttpApi.Client.Clients;

public class PermissionApiClient : ApiClientBase
{
    private const string BaseUrl = "api/v1/identity/permissions";

    public PermissionApiClient(HttpClient httpClient, IApiErrorHandler errorHandler)
        : base(httpClient, errorHandler)
    {
    }

    public async Task<List<IdentityPermissionGrantDto>?> GetAsync(GetPermissionListInput input, Guid? tenantId = null)
    {
        var queryString = new Dictionary<string, string?>
        {
            ["ProviderName"] = input.ProviderName,
            ["ProviderKey"] = input.ProviderKey
        };

        var url = QueryHelpers.AddQueryString(BaseUrl, queryString);
        var response = await GetAsync<List<IdentityPermissionGrantDto>>(url, req =>
        {
            if (tenantId.HasValue)
            {
                req.Headers.Add(CustomHttpHeaderNames.TenantId, tenantId.Value.ToString());
            }
        });
        return response.Result.Data;
    }

    public async Task UpdateAsync(UpdatePermissionsInput input, Guid? tenantId = null)
    {
        await PutAsync<UpdatePermissionsInput, object>(BaseUrl, input, req =>
        {
            if (tenantId.HasValue)
            {
                req.Headers.Add(CustomHttpHeaderNames.TenantId, tenantId.Value.ToString());
            }
        });
    }

    // ── Template Fallback Methods ─────────────────────────

    public async Task<PagedResultDto<PermissionTemplateDto>?> GetTemplatesAsync(GetPermissionTemplatesInput input)
    {
        var queryString = new Dictionary<string, string?>();
        queryString["PageNumber"] = input.PageNumber.ToString();
        queryString["PageSize"] = input.PageSize.ToString();
        if (!string.IsNullOrEmpty(input.SorterKey)) queryString["SorterKey"] = input.SorterKey;
        if (input.SorterIsAsc.HasValue) queryString["SorterIsAsc"] = input.SorterIsAsc.Value.ToString();
        if (!string.IsNullOrEmpty(input.Filter)) queryString["Filter"] = input.Filter;

        var url = QueryHelpers.AddQueryString("api/v1/permission-management/templates", queryString);
        var response = await GetAsync<PagedResultDto<PermissionTemplateDto>>(url);
        return response.Result.Data;
    }
}

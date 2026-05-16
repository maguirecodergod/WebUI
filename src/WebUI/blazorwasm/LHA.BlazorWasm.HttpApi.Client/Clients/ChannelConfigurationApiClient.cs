using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.Ddd.Application;
using LHA.Shared.Contracts.Notification;
using LHA.Shared.Domain.Enums.Notification;
using Microsoft.AspNetCore.WebUtilities;

namespace LHA.BlazorWasm.HttpApi.Client.Clients;

public class ChannelConfigurationApiClient : ApiClientBase, IChannelConfigurationAppService
{
    private const string BaseUrl = "api/v1/notification/configurations";

    public ChannelConfigurationApiClient(HttpClient httpClient, IApiErrorHandler errorHandler)
        : base(httpClient, errorHandler)
    {
    }

    public async Task<ChannelConfigurationDto?> GetByChannelAsync(CNotificationChannel channel, Guid? tenantId = null)
    {
        var url = $"{BaseUrl}/channel/{channel}";
        if (tenantId.HasValue)
        {
            url = QueryHelpers.AddQueryString(url, "tenantId", tenantId.Value.ToString());
        }
        var response = await GetAsync<ChannelConfigurationDto>(url);
        return response.Result.Data;
    }

    public async Task<ChannelConfigurationDto?> GetAsync(Guid id)
    {
        var url = $"{BaseUrl}/{id}";
        var response = await GetAsync<ChannelConfigurationDto>(url);
        return response.Result.Data;
    }

    public async Task<PagedResultDto<ChannelConfigurationDto>> GetPagedListAsync(GetChannelConfigurationsInput input)
    {
        var url = BuildQueryString(BaseUrl, input);
        var response = await GetAsync<PagedResultDto<ChannelConfigurationDto>>(url);
        return response.Result.Data!;
    }

    public async Task<List<ChannelConfigurationDto>> GetListAsync(Guid? tenantId = null)
    {
        // Note: The backend endpoint for GetList might not exist or might be mapped differently.
        // For now, we use the paged list with a large page size if needed, 
        // or we can just use the base URL if it returns a list.
        // Assuming GetPagedList is the primary way.
        var result = await GetPagedListAsync(new GetChannelConfigurationsInput { PageSize = 1000, TenantId = tenantId });
        return result.Items.ToList();
    }

    public async Task<ChannelConfigurationDto> CreateAsync(CreateUpdateChannelConfigurationDto input, Guid? tenantId = null)
    {
        var url = BaseUrl;
        if (tenantId.HasValue)
        {
            url = QueryHelpers.AddQueryString(url, "tenantId", tenantId.Value.ToString());
        }
        var response = await PostAsync<CreateUpdateChannelConfigurationDto, ChannelConfigurationDto>(url, input);
        return response.Result.Data!;
    }

    public async Task<ChannelConfigurationDto> UpdateAsync(Guid id, CreateUpdateChannelConfigurationDto input)
    {
        var response = await PutAsync<CreateUpdateChannelConfigurationDto, ChannelConfigurationDto>($"{BaseUrl}/{id}", input);
        return response.Result.Data!;
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteAsync<object>($"{BaseUrl}/{id}");
    }

    public async Task SetEnabledAsync(Guid id, bool isEnabled)
    {
        var action = isEnabled ? "enable" : "disable";
        await PostAsync<object, object>($"{BaseUrl}/{id}/{action}", new { });
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

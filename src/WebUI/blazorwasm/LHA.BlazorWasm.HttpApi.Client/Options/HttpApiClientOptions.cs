namespace LHA.BlazorWasm.HttpApi.Client.Options;

/// <summary>
/// Configuration options for the API client, commonly populated from appsettings.json.
/// </summary>
public class HttpApiClientOptions
{
    public const string SectionName = "HttpApiClient";

    public string BaseAddress { get; set; } = string.Empty;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxRetries { get; set; } = 3;
}

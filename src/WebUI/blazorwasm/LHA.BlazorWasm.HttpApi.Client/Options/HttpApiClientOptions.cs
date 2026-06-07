namespace LHA.BlazorWasm.HttpApi.Client.Options;

/// <summary>
/// Configuration options for the API client, commonly populated from appsettings.json.
/// </summary>
public class HttpApiClientOptions
{
    /// <summary>
    /// The section name in appsettings.json for API client options.
    /// </summary>
    public const string SectionName = "HttpApiClient";
    /// <summary>
    /// The base address for API requests.
    /// </summary>
    public string BaseAddress { get; set; } = string.Empty;
    /// <summary>
    /// The timeout for API requests.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    /// <summary>
    /// The maximum number of retries for failed requests.
    /// </summary>
    public int MaxRetries { get; set; } = 3;
}

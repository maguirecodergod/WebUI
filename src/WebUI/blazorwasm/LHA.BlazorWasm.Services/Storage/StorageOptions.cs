namespace LHA.BlazorWasm.Services.Storage;

/// <summary>
/// Configuration options for the Local Storage service abstraction.
/// </summary>
public class StorageOptions
{
    /// <summary>
    /// The prefix used for all stored keys (e.g., "app:").
    /// </summary>
    public string KeyPrefix { get; set; } = "app:";
}

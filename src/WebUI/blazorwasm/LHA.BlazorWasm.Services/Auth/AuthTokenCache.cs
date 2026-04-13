namespace LHA.BlazorWasm.Services.Auth;

/// <summary>
/// Singleton cache for the authentication token to prevent JS interop queues
/// when multiple HTTP requests are made simultaneously.
/// </summary>
public class AuthTokenCache
{
    public string? AccessToken { get; set; }
    public bool IsInitialized { get; set; }

    public void SetToken(string? token)
    {
        AccessToken = token;
        IsInitialized = true;
    }

    public void Clear()
    {
        AccessToken = null;
        IsInitialized = true;
    }
}

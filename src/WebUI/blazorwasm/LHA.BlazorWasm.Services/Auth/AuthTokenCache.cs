namespace LHA.BlazorWasm.Services.Auth;

/// <summary>
/// Singleton cache for the authentication token to prevent JS interop queues
/// when multiple HTTP requests are made simultaneously.
/// </summary>
public class AuthTokenCache
{
    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }
    public bool IsInitialized { get; private set; }

    public void SetToken(string? accessToken, string? refreshToken = null, long expiresInSeconds = 0)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        
        if (expiresInSeconds > 0)
        {
            ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds);
        }
        else
        {
            ExpiresAt = null;
        }

        IsInitialized = true;
    }

    public void Clear()
    {
        AccessToken = null;
        RefreshToken = null;
        ExpiresAt = null;
        IsInitialized = true;
    }
}

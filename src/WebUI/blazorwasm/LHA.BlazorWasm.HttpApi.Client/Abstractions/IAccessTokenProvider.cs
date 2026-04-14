namespace LHA.BlazorWasm.HttpApi.Client.Abstractions;

/// <summary>
/// Provides access to the current access token for authentication headers.
/// </summary>
public interface IAccessTokenProvider
{
    /// <summary>
    /// Retrieves the access token synchronously or asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The access token as a string, or null if unauthenticated.</returns>
    ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Forces a token refresh even if the current token hasn't expired yet.
    /// </summary>
    /// <param name="failedToken">The token that caused the 401 Unauthorized, to prevent concurrent redundant refreshes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The new access token, or null if refresh failed.</returns>
    ValueTask<string?> ForceRefreshAsync(string? failedToken = null, CancellationToken cancellationToken = default);
}

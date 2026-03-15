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
}

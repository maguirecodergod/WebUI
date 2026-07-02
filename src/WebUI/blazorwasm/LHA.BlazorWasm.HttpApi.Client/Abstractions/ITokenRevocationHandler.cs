namespace LHA.BlazorWasm.HttpApi.Client.Abstractions;

/// <summary>
/// Defines a contract for handling token revocation events in the application.
/// </summary>
public interface ITokenRevocationHandler
{
    /// <summary>
    /// Handles the token revocation event, allowing for custom logic to be executed when a token is revoked.
    /// This could include clearing cached data, logging the event, or notifying other parts of the application about the revocation.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleRevocationAsync(CancellationToken cancellationToken = default);
}


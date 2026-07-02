namespace LHA.BlazorWasm.Services.Auth;

public interface ISecurityRevocationUiService
{
    SecurityRevocationState State { get; }

    Task RequestGracefulRevocationAsync(CancellationToken cancellationToken = default);

    Task ConfirmLogoutAsync(CancellationToken cancellationToken = default);
}

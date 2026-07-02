using LHA.BlazorWasm.HttpApi.Client.Abstractions;

namespace LHA.BlazorWasm.Services.Auth;

public sealed class SecurityRevocationHandler(ISecurityRevocationUiService revocationUi)
    : ITokenRevocationHandler
{
    public Task HandleRevocationAsync(CancellationToken cancellationToken = default)
        => revocationUi.RequestGracefulRevocationAsync(cancellationToken);
}

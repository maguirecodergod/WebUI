namespace LHA.Shared.Contracts.Security;

public sealed class OAuthErrorDto
{
    public string Error { get; init; } = "invalid_grant";

    public string ErrorDescription { get; init; } = SecurityRevocationConstants.ErrorDescription;
}


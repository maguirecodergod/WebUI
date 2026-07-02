namespace LHA.Shared.Contracts.Security;

public sealed class SecurityVersionExpiredException : UnauthorizedAccessException
{
    public SecurityVersionExpiredException()
        : base(SecurityRevocationConstants.ErrorDescription)
    {
    }
}


using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Auth;

/// <summary>Input for refreshing tokens.</summary>
public sealed class RefreshTokenInput
{
    public required string RefreshToken { get; init; }
}

public sealed class RefreshTokenInputValidator : AbstractValidator<RefreshTokenInput>
{
    public RefreshTokenInputValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

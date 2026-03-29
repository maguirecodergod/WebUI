using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Auth;

/// <summary>Input for login.</summary>
public sealed class LoginInput
{
    public required string UserNameOrEmail { get; init; }
    public required string Password { get; init; }
}

public sealed class LoginInputValidator : AbstractValidator<LoginInput>
{
    public LoginInputValidator()
    {
        RuleFor(x => x.UserNameOrEmail).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

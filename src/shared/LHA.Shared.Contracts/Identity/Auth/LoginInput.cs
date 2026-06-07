using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Auth;

/// <summary>Input for authenticating a user via login credentials.</summary>
public sealed class LoginInput
{
    /// <summary>Username or email address used to identify the account.</summary>
    public string UserNameOrEmail { get; set; } = string.Empty;

    /// <summary>Plaintext password supplied by the user.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Indicates whether to issue a persistent cookie that survives browser restarts.</summary>
    public bool RememberMe { get; set; } = false;
}

public sealed class LoginInputValidator : AbstractValidator<LoginInput>
{
    public LoginInputValidator()
    {
        RuleFor(x => x.UserNameOrEmail)
            .NotEmpty()
            .WithMessage("Login.Validation.UserNameOrEmailRequired");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Login.Validation.PasswordRequired");
    }
}

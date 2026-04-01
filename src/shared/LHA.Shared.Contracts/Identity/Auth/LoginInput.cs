using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Auth;

/// Professional Login Input for both UI and API contracts.
/// Uses FluentValidation for enterprise-grade validation.
/// </summary>
public sealed class LoginInput
{
    public string UserNameOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
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

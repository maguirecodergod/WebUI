using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Users;

/// <summary>Input for changing a user's password.</summary>
public sealed class ChangePasswordInput
{
    public required string CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
}

public sealed class ChangePasswordInputValidator : AbstractValidator<ChangePasswordInput>
{
    public ChangePasswordInputValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6);
    }
}

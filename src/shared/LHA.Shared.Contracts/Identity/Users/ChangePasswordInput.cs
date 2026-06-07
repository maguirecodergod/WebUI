using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Users;

/// <summary>Input for changing a user's password.</summary>
public sealed class ChangePasswordInput
{
    /// <summary>Current password of the user, required to authorize the change.</summary>
    public required string CurrentPassword { get; init; }

    /// <summary>Desired new password. Minimum length: 6 characters.</summary>
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

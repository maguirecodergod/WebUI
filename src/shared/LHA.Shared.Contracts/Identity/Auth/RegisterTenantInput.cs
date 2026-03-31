using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Auth;

/// <summary>Input for registering a new tenant (onboarding).</summary>
public sealed class RegisterTenantInput
{
    public string TenantName { get; set; } = string.Empty;
    public string AdminUserName { get; set; } = "admin";
    public string AdminEmail { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;

    /// <summary>
    /// Database isolation style (1=Shared, 2=PerTenant, 3=Hybrid).
    /// </summary>
    public int DatabaseStyle { get; set; } = 1;
}

public sealed class RegisterTenantInputValidator : AbstractValidator<RegisterTenantInput>
{
    public RegisterTenantInputValidator()
    {
        RuleFor(x => x.TenantName)
            .NotEmpty().WithMessage("TenantRegister.Validation.NameRequired")
            .MaximumLength(128);

        RuleFor(x => x.AdminUserName)
            .NotEmpty().WithMessage("TenantRegister.Validation.AdminUsernameRequired")
            .MaximumLength(256);

        RuleFor(x => x.AdminEmail)
            .NotEmpty().WithMessage("TenantRegister.Validation.AdminEmailRequired")
            .EmailAddress().WithMessage("TenantRegister.Validation.InvalidEmail")
            .MaximumLength(256);

        RuleFor(x => x.AdminPassword)
            .NotEmpty().WithMessage("TenantRegister.Validation.AdminPasswordRequired")
            .MinimumLength(6).WithMessage("TenantRegister.Validation.PasswordTooShort");

        RuleFor(x => x.DatabaseStyle).InclusiveBetween(1, 3);
    }
}

using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Auth;

/// <summary>Input for registering a new tenant (onboarding).</summary>
public sealed class RegisterTenantInput
{
    public required string TenantName { get; init; }
    public required string AdminUserName { get; init; }
    public required string AdminEmail { get; init; }
    public required string AdminPassword { get; init; }

    /// <summary>
    /// Database isolation style (1=Shared, 2=PerTenant, 3=Hybrid).
    /// </summary>
    public int DatabaseStyle { get; init; } = 1;
}

public sealed class RegisterTenantInputValidator : AbstractValidator<RegisterTenantInput>
{
    public RegisterTenantInputValidator()
    {
        RuleFor(x => x.TenantName).NotEmpty().MaximumLength(128);
        RuleFor(x => x.AdminUserName).NotEmpty().MaximumLength(256);
        RuleFor(x => x.AdminEmail).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.AdminPassword).NotEmpty().MinimumLength(6);
        RuleFor(x => x.DatabaseStyle).InclusiveBetween(1, 3);
    }
}

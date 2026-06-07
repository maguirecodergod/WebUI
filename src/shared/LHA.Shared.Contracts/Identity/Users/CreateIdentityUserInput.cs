using LHA.Shared.Contracts.Identity;
using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Users;

/// <summary>Input for creating a new user.</summary>
public sealed class CreateIdentityUserInput
{
    /// <summary>Unique login name for the new user. Maximum length: 256 characters.</summary>
    public required string UserName { get; set; }

    /// <summary>Primary email address for the new user. Maximum length: 256 characters.</summary>
    public required string Email { get; set; }

    /// <summary>Initial password for the new user. Minimum length: 6 characters.</summary>
    public required string Password { get; set; }

    /// <summary>Phone number for the new user, if provided. Maximum length: 32 characters.</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>Given (first) name of the new user. Maximum length: 64 characters.</summary>
    public string? Name { get; set; }

    /// <summary>Family (last) name of the new user. Maximum length: 64 characters.</summary>
    public string? Surname { get; set; }

    /// <summary>Indicates whether the account is subject to lockout after repeated failed login attempts. Defaults to <c>true</c>.</summary>
    public bool LockoutEnabled { get; set; } = true;

    /// <summary>Identifiers of roles to assign to the new user upon creation.</summary>
    public List<Guid> RoleIds { get; set; } = [];
}

public sealed class CreateIdentityUserInputValidator : AbstractValidator<CreateIdentityUserInput>
{
    public CreateIdentityUserInputValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.PhoneNumber).MaximumLength(32);
        RuleFor(x => x.Name).MaximumLength(64);
        RuleFor(x => x.Surname).MaximumLength(64);
    }
}

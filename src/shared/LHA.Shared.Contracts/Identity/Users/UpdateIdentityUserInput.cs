using LHA.Shared.Contracts.Identity;
using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Users;

/// <summary>Input for updating an existing user.</summary>
public sealed class UpdateIdentityUserInput
{
    /// <summary>Updated login name of the user. Maximum length: 256 characters.</summary>
    public required string UserName { get; init; }

    /// <summary>Updated email address of the user. Maximum length: 256 characters.</summary>
    public required string Email { get; init; }

    /// <summary>Updated phone number of the user. Maximum length: 32 characters.</summary>
    public string? PhoneNumber { get; init; }

    /// <summary>Updated given (first) name of the user. Maximum length: 64 characters.</summary>
    public string? Name { get; init; }

    /// <summary>Updated family (last) name of the user. Maximum length: 64 characters.</summary>
    public string? Surname { get; init; }

    /// <summary>Indicates whether the account is subject to lockout after repeated failed login attempts. Defaults to <c>true</c>.</summary>
    public bool LockoutEnabled { get; init; } = true;

    /// <summary>Complete set of role identifiers to assign to the user; replaces any existing assignments.</summary>
    public List<Guid> RoleIds { get; init; } = [];

    /// <summary>Concurrency token from the last read, used for optimistic locking.</summary>
    public required string ConcurrencyStamp { get; init; }
}

public sealed class UpdateIdentityUserInputValidator : AbstractValidator<UpdateIdentityUserInput>
{
    public UpdateIdentityUserInputValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.PhoneNumber).MaximumLength(32);
        RuleFor(x => x.Name).MaximumLength(64);
        RuleFor(x => x.Surname).MaximumLength(64);
        RuleFor(x => x.ConcurrencyStamp).NotEmpty();
    }
}

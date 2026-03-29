using LHA.Identity.Domain.Shared;
using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Users;

/// <summary>Input for updating an existing user.</summary>
public sealed class UpdateIdentityUserInput
{
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Name { get; init; }
    public string? Surname { get; init; }
    public bool LockoutEnabled { get; init; } = true;
    public List<Guid> RoleIds { get; init; } = [];
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

using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Roles;

/// <summary>Input for creating a new role.</summary>
public sealed class CreateIdentityRoleInput
{
    public required string Name { get; init; }
    public bool IsDefault { get; init; }
    public bool IsPublic { get; init; } = true;
}

public sealed class CreateIdentityRoleInputValidator : AbstractValidator<CreateIdentityRoleInput>
{
    public CreateIdentityRoleInputValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
    }
}

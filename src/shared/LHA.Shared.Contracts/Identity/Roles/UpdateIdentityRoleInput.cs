using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Roles;

/// <summary>Input for updating an existing role.</summary>
public sealed class UpdateIdentityRoleInput
{
    public required string Name { get; init; }
    public bool IsDefault { get; init; }
    public bool IsPublic { get; init; } = true;
    public required string ConcurrencyStamp { get; init; }
}

public sealed class UpdateIdentityRoleInputValidator : AbstractValidator<UpdateIdentityRoleInput>
{
    public UpdateIdentityRoleInputValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
        RuleFor(x => x.ConcurrencyStamp).NotEmpty();
    }
}

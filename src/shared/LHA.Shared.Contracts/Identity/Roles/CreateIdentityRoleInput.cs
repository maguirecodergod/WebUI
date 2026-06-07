using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Roles;

/// <summary>Input for creating a new role.</summary>
public sealed class CreateIdentityRoleInput
{
    /// <summary>Display name of the new role. Maximum length: 256 characters.</summary>
    public required string Name { get; init; }

    /// <summary>Indicates whether the role is automatically assigned to newly registered users.</summary>
    public bool IsDefault { get; init; }

    /// <summary>Indicates whether the role is visible to tenant administrators in the UI. Defaults to <c>true</c>.</summary>
    public bool IsPublic { get; init; } = true;
}

public sealed class CreateIdentityRoleInputValidator : AbstractValidator<CreateIdentityRoleInput>
{
    public CreateIdentityRoleInputValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
    }
}

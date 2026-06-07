using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Roles;

/// <summary>Input for updating an existing role.</summary>
public class UpdateIdentityRoleInput
{
    /// <summary>Updated display name of the role. Maximum length: 256 characters.</summary>
    public required string Name { get; init; }

    /// <summary>Indicates whether the role is automatically assigned to newly registered users.</summary>
    public bool IsDefault { get; init; }

    /// <summary>Indicates whether the role is visible to tenant administrators in the UI. Defaults to <c>true</c>.</summary>
    public bool IsPublic { get; init; } = true;

    /// <summary>Concurrency token from the last read, used for optimistic locking.</summary>
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

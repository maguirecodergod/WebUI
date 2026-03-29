using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Permissions;

/// <summary>A single permission grant/revoke entry.</summary>
public sealed class PermissionGrantInput
{
    public required string Name { get; init; }
    public bool IsGranted { get; init; }
}

public sealed class PermissionGrantInputValidator : AbstractValidator<PermissionGrantInput>
{
    public PermissionGrantInputValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}

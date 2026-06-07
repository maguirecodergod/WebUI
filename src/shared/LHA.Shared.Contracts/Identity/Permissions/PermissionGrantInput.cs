using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Permissions;

/// <summary>A single permission grant/revoke entry.</summary>
public sealed class PermissionGrantInput
{
    /// <summary>Unique name of the permission (e.g. <c>"Identity.Users.Create"</c>).</summary>
    public required string Name { get; set; }

    /// <summary>Indicates whether the permission is granted (<c>true</c>) or revoked (<c>false</c>).</summary>
    public bool IsGranted { get; set; }
}

public sealed class PermissionGrantInputValidator : AbstractValidator<PermissionGrantInput>
{
    public PermissionGrantInputValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}

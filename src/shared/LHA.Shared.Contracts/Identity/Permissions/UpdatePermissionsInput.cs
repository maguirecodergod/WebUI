using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Permissions;

/// <summary>Input for granting or revoking a set of permissions.</summary>
public sealed class UpdatePermissionsInput
{
    public required string ProviderName { get; init; }
    public required string ProviderKey { get; init; }
    public List<PermissionGrantInput> Permissions { get; init; } = [];
}

public sealed class UpdatePermissionsInputValidator : AbstractValidator<UpdatePermissionsInput>
{
    public UpdatePermissionsInputValidator()
    {
        RuleFor(x => x.ProviderName).NotEmpty();
        RuleFor(x => x.ProviderKey).NotEmpty();
        RuleForEach(x => x.Permissions).SetValidator(new PermissionGrantInputValidator());
    }
}

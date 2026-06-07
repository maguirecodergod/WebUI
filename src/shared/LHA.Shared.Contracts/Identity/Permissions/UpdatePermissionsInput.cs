using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Permissions;

/// <summary>Input for granting or revoking a set of permissions.</summary>
public sealed class UpdatePermissionsInput
{
    /// <summary>Category of the provider whose permissions are being updated (e.g. <c>"R"</c> for role, <c>"U"</c> for user).</summary>
    public required string ProviderName { get; init; }

    /// <summary>Identifier of the specific provider instance (e.g. the role or user id).</summary>
    public required string ProviderKey { get; init; }

    /// <summary>Collection of permission grant/revoke entries to apply.</summary>
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

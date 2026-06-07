using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Permissions;

/// <summary>Input for querying permissions for a provider.</summary>
public sealed class GetPermissionListInput
{
    /// <summary>Category of the provider (e.g. <c>"R"</c> for role, <c>"U"</c> for user).</summary>
    public required string ProviderName { get; init; }

    /// <summary>Identifier of the specific provider instance (e.g. the role or user id).</summary>
    public required string ProviderKey { get; init; }
}

public sealed class GetPermissionListInputValidator : AbstractValidator<GetPermissionListInput>
{
    public GetPermissionListInputValidator()
    {
        RuleFor(x => x.ProviderName).NotEmpty();
        RuleFor(x => x.ProviderKey).NotEmpty();
    }
}

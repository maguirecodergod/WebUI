using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Permissions;

/// <summary>Input for querying permissions for a provider.</summary>
public sealed class GetPermissionListInput
{
    public required string ProviderName { get; init; }
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

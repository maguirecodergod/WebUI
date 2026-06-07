using LHA.Shared.Contracts.Identity;
using FluentValidation;

using LHA.Shared.Domain.Identity;
namespace LHA.Shared.Contracts.Identity.Claims;


/// <summary>Input for creating or updating a claim type.</summary>
public sealed class CreateOrUpdateClaimTypeInput
{
    /// <summary>Unique name of the claim type. Maximum length: 256 characters.</summary>
    public required string Name { get; init; }

    /// <summary>Indicates whether the claim must be provided by every user.</summary>
    public bool Required { get; init; }

    /// <summary>Expected data type of the claim value. Defaults to <see cref="CIdentityClaimValueType.String"/>.</summary>
    public CIdentityClaimValueType ValueType { get; init; } = CIdentityClaimValueType.String;

    /// <summary>Regular expression used to validate the claim value. Maximum length: 512 characters.</summary>
    public string? Regex { get; init; }

    /// <summary>Human-readable description of the validation regex pattern. Maximum length: 512 characters.</summary>
    public string? RegexDescription { get; init; }

    /// <summary>Descriptive text explaining the purpose of the claim type. Maximum length: 512 characters.</summary>
    public string? Description { get; init; }
}

public sealed class CreateOrUpdateClaimTypeInputValidator : AbstractValidator<CreateOrUpdateClaimTypeInput>
{
    public CreateOrUpdateClaimTypeInputValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Description).MaximumLength(512);
        RuleFor(x => x.Regex).MaximumLength(512);
        RuleFor(x => x.RegexDescription).MaximumLength(512);
    }
}

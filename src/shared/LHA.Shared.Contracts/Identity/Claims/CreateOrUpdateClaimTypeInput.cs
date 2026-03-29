using LHA.Identity.Domain.Shared;
using FluentValidation;

namespace LHA.Shared.Contracts.Identity.Claims;

/// <summary>Input for creating or updating a claim type.</summary>
public sealed class CreateOrUpdateClaimTypeInput
{
    public required string Name { get; init; }
    public bool Required { get; init; }
    public CIdentityClaimValueType ValueType { get; init; } = CIdentityClaimValueType.String;
    public string? Regex { get; init; }
    public string? RegexDescription { get; init; }
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

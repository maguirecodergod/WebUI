using LHA.Ddd.Domain;
using LHA.Identity.Domain.Shared;
using LHA.Shared.Domain.Identity;

namespace LHA.Identity.Domain;

/// <summary>
/// Defines the metadata of a claim type used in the identity system.
/// Allows administrators to define which claims exist and their validation rules.
/// </summary>
public sealed class IdentityClaimType : FullAuditedAggregateRoot<Guid>
{
    /// <summary>Display / logical name of the claim type (unique).</summary>
    public string Name { get; private set; } = null!;

    /// <summary>Whether the claim is required for user creation.</summary>
    public bool Required { get; private set; }

    /// <summary>Whether the claim type is system-defined and cannot be deleted.</summary>
    public bool IsStatic { get; private set; }

    /// <summary>Expected value type.</summary>
    public CIdentityClaimValueType ValueType { get; private set; }

    /// <summary>Regex pattern for value validation.</summary>
    public string? Regex { get; private set; }

    /// <summary>Human-readable description of the regex pattern.</summary>
    public string? RegexDescription { get; private set; }

    /// <summary>Description of the claim type.</summary>
    public string? Description { get; private set; }

    /// <summary>EF Core constructor.</summary>
    private IdentityClaimType() { }

    /// <summary>Creates a new claim type definition.</summary>
    public IdentityClaimType(
        Guid id,
        string name,
        bool required = false,
        bool isStatic = false,
        CIdentityClaimValueType valueType = CIdentityClaimValueType.String,
        string? regex = null,
        string? regexDescription = null,
        string? description = null)
    {
        Id = id;
        SetNameInternal(name);
        Required = required;
        IsStatic = isStatic;
        ValueType = valueType;
        Regex = regex;
        RegexDescription = regexDescription;
        Description = description;
    }

    /// <summary>Updates the claim type.</summary>
    public IdentityClaimType Update(
        string name,
        bool required,
        CIdentityClaimValueType valueType,
        string? regex,
        string? regexDescription,
        string? description)
    {
        if (IsStatic)
            throw new BusinessException(IdentityDomainErrorCodes.CannotModifyStaticClaimType)
                .WithData(Name);

        SetNameInternal(name);
        Required = required;
        ValueType = valueType;
        Regex = regex;
        RegexDescription = regexDescription;
        Description = description;
        return this;
    }

    private void SetNameInternal(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (name.Length > IdentityClaimTypeConsts.MaxNameLength)
            throw new ArgumentException(
                $"Claim type name must not exceed {IdentityClaimTypeConsts.MaxNameLength} characters.",
                nameof(name));
        Name = name.Trim();
    }
}

using LHA.Identity.Domain;

namespace LHA.Identity.Application;

/// <summary>
/// Normalizes names and emails to upper-invariant form for case-insensitive lookups.
/// </summary>
public sealed class UpperInvariantLookupNormalizer : ILookupNormalizer
{
    /// <inheritdoc />
    public string NormalizeName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return name.Trim().ToUpperInvariant();
    }

    /// <inheritdoc />
    public string NormalizeEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        return email.Trim().ToUpperInvariant();
    }
}

namespace LHA.Caching;

/// <summary>
/// Lightweight DTO for caching user lookup results.
/// <para>
/// This is a serializable snapshot of user identity info, stored in
/// the cache so that repeated lookups (e.g., resolving creator names
/// for audit log display) don't hit the database.
/// </para>
/// </summary>
[CacheName("u")]
public sealed class CachedUserItem
{
    /// <summary>User identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Login name.</summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>Display name (first name).</summary>
    public string? Name { get; set; }

    /// <summary>Surname (last name).</summary>
    public string? Surname { get; set; }

    /// <summary>Email address.</summary>
    public string? Email { get; set; }

    /// <summary>Tenant identifier.</summary>
    public Guid? TenantId { get; set; }

    /// <summary>Comma-separated role names.</summary>
    public string[] Roles { get; set; } = [];

    /// <summary>
    /// Full display name (Name + Surname, or UserName as fallback).
    /// </summary>
    public string GetFullName()
    {
        if (string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(Surname))
            return UserName;

        return string.Join(' ',
            new[] { Name, Surname }
                .Where(s => !string.IsNullOrWhiteSpace(s)));
    }
}

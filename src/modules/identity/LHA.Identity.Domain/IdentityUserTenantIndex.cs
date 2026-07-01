using LHA.Auditing;
using LHA.Ddd.Domain;

namespace LHA.Identity.Domain;

/// <summary>
/// Central index table for fast username/email → tenant lookup.
/// Enables cross-tenant login without querying every tenant database.
/// <para>
/// This entity lives in the HOST database (IdentityDbContext) and is
/// maintained whenever a user is created, updated, or deleted.
/// </para>
/// </summary>
public sealed class IdentityUserTenantIndex : Entity<Guid>, ICreationAuditedObject
{
    /// <summary>
    /// The tenant this user belongs to.
    /// </summary>
    public Guid? TenantId { get; private set; }

    /// <summary>
    /// Normalized username (uppercase) for case-insensitive lookup.
    /// Must be unique across all tenants.
    /// </summary>
    public string NormalizedUserName { get; private set; } = null!;

    /// <summary>
    /// Normalized email (uppercase) for case-insensitive lookup.
    /// Must be unique across all tenants.
    /// </summary>
    public string NormalizedEmail { get; private set; } = null!;

    /// <summary>
    /// Reference to the actual user record.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreationTime { get; set; }

    /// <inheritdoc />
    public Guid? CreatorId { get; set; }

    /// <summary>
    /// EF Core constructor.
    /// </summary>
    private IdentityUserTenantIndex() { }

    /// <summary>
    /// Creates a new IdentityUserTenantIndex entry.
    /// </summary>
    public IdentityUserTenantIndex(
        string normalizedUserName,
        string normalizedEmail,
        Guid userId,
        Guid? tenantId)
    {
        Id = Guid.CreateVersion7();
        NormalizedUserName = normalizedUserName;
        NormalizedEmail = normalizedEmail;
        UserId = userId;
        TenantId = tenantId;
        CreationTime = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Updates the normalized fields (called when user email/username changes).
    /// </summary>
    public void UpdateNormalizedFields(string normalizedUserName, string normalizedEmail)
    {
        NormalizedUserName = normalizedUserName;
        NormalizedEmail = normalizedEmail;
    }

    /// <summary>
    /// Updates the tenant reference (called when user is moved to another tenant).
    /// </summary>
    public void UpdateTenant(Guid? tenantId)
    {
        TenantId = tenantId;
    }
}

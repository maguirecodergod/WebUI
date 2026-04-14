namespace LHA.Core.Security;

/// <summary>
/// Standard and custom claim types used throughout the LHA platform.
/// <para>
/// JWT tokens issued by the Account Service should include these claim types
/// so that <see cref="HttpContextCurrentUser"/> can read them consistently.
/// </para>
/// </summary>
public static class LhaClaimTypes
{
    // ─── Standard JWT claims ────────────────────────────────────
    /// <summary>Subject (user ID). Standard JWT "sub" claim.</summary>
    public const string Subject = "sub";

    /// <summary>Preferred username. Standard OIDC claim.</summary>
    public const string PreferredUserName = "preferred_username";

    /// <summary>Given name (first name). Standard OIDC claim.</summary>
    public const string GivenName = "given_name";

    /// <summary>Family name (surname). Standard OIDC claim.</summary>
    public const string FamilyName = "family_name";

    /// <summary>Email address. Standard OIDC claim.</summary>
    public const string Email = "email";

    /// <summary>Email verified flag. Standard OIDC claim.</summary>
    public const string EmailVerified = "email_verified";

    /// <summary>Phone number. Standard OIDC claim.</summary>
    public const string PhoneNumber = "phone_number";

    /// <summary>Phone number verified flag. Standard OIDC claim.</summary>
    public const string PhoneNumberVerified = "phone_number_verified";

    // ─── Custom LHA claims ──────────────────────────────────────
    /// <summary>Tenant identifier. Custom claim added by the Account Service.</summary>
    public const string TenantId = "tenant_id";

    /// <summary>Role. Uses the standard .NET role claim type for AuthorizeAttribute compat.</summary>
    public const string Role = "role";

    /// <summary>Permission claims for fine-grained authorization.</summary>
    public const string Permission = "permissions";

    /// <summary>Impersonator user ID (admin-as-user scenarios).</summary>
    public const string ImpersonatorUserId = "impersonator_user_id";

    /// <summary>Impersonator tenant ID.</summary>
    public const string ImpersonatorTenantId = "impersonator_tenant_id";

    /// <summary>Security stamp for token invalidation validation.</summary>
    public const string SecurityStamp = "security_stamp";

    /// <summary>Client identifier (API key / OAuth client).</summary>
    public const string ClientId = "client_id";
}

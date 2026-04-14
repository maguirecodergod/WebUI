using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using LHA.Core.Security;

namespace LHA.Identity.Application;

// ─── Options ─────────────────────────────────────────────────────────

/// <summary>
/// Configuration options for JWT token generation.
/// </summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    /// <summary>Token issuer (iss claim).</summary>
    public required string Issuer { get; init; }

    /// <summary>Token audience (aud claim).</summary>
    public required string Audience { get; init; }

    /// <summary>HMAC-SHA256 secret key (≥ 32 chars recommended).</summary>
    public required string SecretKey { get; init; }

    /// <summary>Access token lifetime in minutes (default 30).</summary>
    public int AccessTokenExpirationMinutes { get; init; } = 30;

    /// <summary>Refresh token lifetime in days (default 7).</summary>
    public int RefreshTokenExpirationDays { get; init; } = 7;
}

// ─── Service ─────────────────────────────────────────────────────────

/// <summary>
/// Generates and validates JWT access tokens and opaque refresh tokens.
/// </summary>
public sealed class JwtTokenService
{
    public const string RefreshTokenProvider = "LHA.Identity";
    public const string RefreshTokenName = "RefreshToken";

    private readonly JwtOptions _options;
    private readonly JsonWebTokenHandler _handler = new();

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Generates a signed JWT access token containing user claims.
    /// </summary>
    public string GenerateAccessToken(
        Guid userId,
        string userName,
        string email,
        Guid? tenantId,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new Dictionary<string, object>
        {
            [LhaClaimTypes.Subject] = userId.ToString(),
            [JwtRegisteredClaimNames.Jti] = Guid.CreateVersion7().ToString(),
            [LhaClaimTypes.PreferredUserName] = userName,
            [LhaClaimTypes.Email] = email,
        };

        if (tenantId.HasValue)
            claims[LhaClaimTypes.TenantId] = tenantId.Value.ToString();

        if (roles.Count > 0)
            claims[LhaClaimTypes.Role] = roles.ToArray();

        if (permissions.Count > 0)
            claims[LhaClaimTypes.Permission] = permissions.ToArray();

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Claims = claims,
            Expires = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes),
            SigningCredentials = credentials,
        };

        return _handler.CreateToken(descriptor);
    }

    /// <summary>
    /// Generates a cryptographically-random opaque refresh token.
    /// </summary>
    public static string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    /// <summary>
    /// Returns the expiration time for a new refresh token.
    /// </summary>
    public DateTimeOffset GetRefreshTokenExpiration()
        => DateTimeOffset.UtcNow.AddDays(_options.RefreshTokenExpirationDays);

    /// <summary>
    /// Access token expiration in seconds (used in API responses).
    /// </summary>
    public long AccessTokenExpiresInSeconds
        => _options.AccessTokenExpirationMinutes * 60L;
}

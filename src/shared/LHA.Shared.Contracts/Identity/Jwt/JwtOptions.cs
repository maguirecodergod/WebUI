namespace LHA.Shared.Contracts
{
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
}
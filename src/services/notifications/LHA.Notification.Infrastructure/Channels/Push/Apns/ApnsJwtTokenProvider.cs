using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace LHA.Notification.Infrastructure.Channels.Push.Apns;

public sealed class ApnsJwtTokenProvider
{
    private readonly ApnsProviderSettings _config;
    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public ApnsJwtTokenProvider(IOptions<ApnsProviderSettings> config)
    {
        _config = config.Value;
    }

    public string GetToken()
    {
        if (_cachedToken != null && DateTime.UtcNow < _tokenExpiry)
            return _cachedToken;

        // In a real scenario, we'd handle the certificate from settings (e.g. Base64 or File path)
        // Here we assume P8Certificate holds the key content or path
        string p8Content = _config.P8Certificate;
        if (File.Exists(p8Content))
        {
            p8Content = File.ReadAllText(p8Content);
        }

        ECDsa ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(p8Content);

        var securityKey = new ECDsaSecurityKey(ecdsa) { KeyId = _config.KeyId };
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.EcdsaSha256);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(
            issuer: _config.TeamId,
            issuedAt: DateTime.UtcNow,
            signingCredentials: credentials);

        _cachedToken = handler.WriteToken(token);
        _tokenExpiry = DateTime.UtcNow.AddMinutes(50);

        return _cachedToken;
    }
}

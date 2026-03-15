using Microsoft.Extensions.Options;

namespace LHA.Security.Options;

public class SecurityOptions
{
    public bool EnableRequestEncryption { get; set; } = true;
    public int MaxRequestAgeSeconds { get; set; } = 30;
    public int KeyRotationSeconds { get; set; } = 300;
    public bool EnableDeviceVerification { get; set; } = true;
    public string? RsaPrivateKeyXml { get; set; }
    public string? RsaPublicKeyXml { get; set; }
}

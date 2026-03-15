using System.Security.Cryptography;
using System.Text;

namespace LHA.Security.Device;

public interface IDeviceFingerprintService
{
    string GenerateFingerprint(string userAgent, string resolution, string timezone, string platform, string language);
}

public class DeviceFingerprintService : IDeviceFingerprintService
{
    public string GenerateFingerprint(string userAgent, string resolution, string timezone, string platform, string language)
    {
        var data = $"{userAgent}|{resolution}|{timezone}|{platform}|{language}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}

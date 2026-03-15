using System.Security.Cryptography;

namespace LHA.Security.Keys;

public record SessionKey(byte[] Key, DateTimeOffset ExpiresAt);

public interface IKeyRotationService
{
    SessionKey GetCurrentKey();
    SessionKey RotateKey();
}

public class KeyRotationService : IKeyRotationService
{
    private SessionKey _currentKey;
    private readonly int _rotationSeconds;

    public KeyRotationService(int rotationSeconds = 300)
    {
        _rotationSeconds = rotationSeconds;
        _currentKey = GenerateNewKey();
    }

    public SessionKey GetCurrentKey()
    {
        if (DateTimeOffset.UtcNow >= _currentKey.ExpiresAt)
        {
            return RotateKey();
        }
        return _currentKey;
    }

    public SessionKey RotateKey()
    {
        _currentKey = GenerateNewKey();
        return _currentKey;
    }

    private SessionKey GenerateNewKey()
    {
        var key = new byte[32]; // AES-256
        RandomNumberGenerator.Fill(key);
        return new SessionKey(key, DateTimeOffset.UtcNow.AddSeconds(_rotationSeconds));
    }
}

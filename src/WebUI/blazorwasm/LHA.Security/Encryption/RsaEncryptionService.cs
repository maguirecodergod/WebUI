using System.Security.Cryptography;

namespace LHA.Security.Encryption;

public interface IRsaEncryptionService
{
    byte[] Encrypt(byte[] data, string publicKeyXml);
    byte[] Decrypt(byte[] ciphertext, string privateKeyXml);
}

public class RsaEncryptionService : IRsaEncryptionService
{
    public byte[] Encrypt(byte[] data, string publicKeyXml)
    {
        using var rsa = RSA.Create();
        rsa.FromXmlString(publicKeyXml);
        return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
    }

    public byte[] Decrypt(byte[] ciphertext, string privateKeyXml)
    {
        using var rsa = RSA.Create();
        rsa.FromXmlString(privateKeyXml);
        return rsa.Decrypt(ciphertext, RSAEncryptionPadding.OaepSHA256);
    }
}

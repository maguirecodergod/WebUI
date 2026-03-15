using System.Security.Cryptography;

namespace LHA.Security.Encryption;

public interface IAesEncryptionService
{
    byte[] Encrypt(byte[] data, byte[] key, out byte[] iv, out byte[] tag);
    byte[] Decrypt(byte[] ciphertext, byte[] key, byte[] iv, byte[] tag);
}

public class AesEncryptionService : IAesEncryptionService
{
    public byte[] Encrypt(byte[] data, byte[] key, out byte[] iv, out byte[] tag)
    {
        iv = new byte[12]; // GCM recommended IV size
        RandomNumberGenerator.Fill(iv);
        tag = new byte[16];
        byte[] ciphertext = new byte[data.Length];

        using var aes = new AesGcm(key, 16);
        aes.Encrypt(iv, data, ciphertext, tag);

        return ciphertext;
    }

    public byte[] Decrypt(byte[] ciphertext, byte[] key, byte[] iv, byte[] tag)
    {
        byte[] plaintext = new byte[ciphertext.Length];

        using var aes = new AesGcm(key, 16);
        aes.Decrypt(iv, ciphertext, tag, plaintext);

        return plaintext;
    }
}

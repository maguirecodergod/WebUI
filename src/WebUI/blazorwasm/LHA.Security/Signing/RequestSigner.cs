using System.Security.Cryptography;
using System.Text;

namespace LHA.Security.Signing;

public interface IRequestSigner
{
    string CreateSignature(string method, string path, string timestamp, string nonce, string bodyHash, byte[] secretKey);
    bool VerifySignature(string signature, string method, string path, string timestamp, string nonce, string bodyHash, byte[] secretKey);
}

public class RequestSigner : IRequestSigner
{
    public string CreateSignature(string method, string path, string timestamp, string nonce, string bodyHash, byte[] secretKey)
    {
        var input = $"{method.ToUpper()}{path}{timestamp}{nonce}{bodyHash}";
        using var hmac = new HMACSHA256(secretKey);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(hash);
    }

    public bool VerifySignature(string signature, string method, string path, string timestamp, string nonce, string bodyHash, byte[] secretKey)
    {
        var computed = CreateSignature(method, path, timestamp, nonce, bodyHash, secretKey);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computed),
            Encoding.UTF8.GetBytes(signature));
    }
}

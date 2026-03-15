using LHA.Security.Encryption;
using LHA.Security.Keys;
using LHA.Security.Signing;
using LHA.Security.Device;
using LHA.BlazorWasm.Shared.Constants;
using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using System.Net.Http.Json;

namespace LHA.BlazorWasm.HttpApi.Client.Handlers;

public class SecureHttpHandler : DelegatingHandler
{
    private readonly IAesEncryptionService _aesService;
    private readonly IKeyRotationService _keyRotation;
    private readonly IRequestSigner _signer;
    private readonly IClientContextProvider _contextProvider;
    private readonly IDeviceFingerprintService _fingerprintService;

    public SecureHttpHandler(
        IAesEncryptionService aesService,
        IKeyRotationService keyRotation,
        IRequestSigner signer,
        IClientContextProvider contextProvider,
        IDeviceFingerprintService fingerprintService)
    {
        _aesService = aesService;
        _keyRotation = keyRotation;
        _signer = signer;
        _contextProvider = contextProvider;
        _fingerprintService = fingerprintService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var nonce = Guid.NewGuid().ToString("N");
        var sessionKey = _keyRotation.GetCurrentKey();
        
        // 0. Inject Device Info
        var deviceId = _contextProvider.GetDeviceId() ?? "UNKNOWN_DEVICE";
        var fingerprint = _fingerprintService.GenerateFingerprint(
            "Mozilla/5.0", "1920x1080", "UTC+7", "Windows", "vi"); // Simplified inputs
            
        request.Headers.Add("X-Device-Id", deviceId);
        request.Headers.Add("X-Device-Fingerprint", fingerprint);


        // 1. Encrypt Body if present
        string bodyHash = "EMPTY";
        if (request.Content != null)
        {
            var rawBody = await request.Content.ReadAsByteArrayAsync(cancellationToken);
            var encryptedBody = _aesService.Encrypt(rawBody, sessionKey.Key, out var iv, out var tag);
            
            // Re-create content as encrypted payload (Simplified for example)
            request.Content = JsonContent.Create(new { 
                data = Convert.ToBase64String(encryptedBody),
                iv = Convert.ToBase64String(iv),
                tag = Convert.ToBase64String(tag)
            });

            using var sha = System.Security.Cryptography.SHA256.Create();
            bodyHash = Convert.ToHexString(sha.ComputeHash(encryptedBody)).ToLowerInvariant();
            
            request.Headers.Add(CustomHttpHeaderNames.RequestEncrypted, "true");
        }

        // 2. Sign Request
        var signature = _signer.CreateSignature(
            request.Method.Method,
            request.RequestUri?.PathAndQuery ?? string.Empty,
            timestamp,
            nonce,
            bodyHash,
            sessionKey.Key);

        // 3. Add Security Headers
        request.Headers.Add(CustomHttpHeaderNames.RequestNonce, nonce);
        request.Headers.Add(CustomHttpHeaderNames.Date, timestamp);
        request.Headers.Add(CustomHttpHeaderNames.RequestSignature, signature);

        return await base.SendAsync(request, cancellationToken);
    }
}

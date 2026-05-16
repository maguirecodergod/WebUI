using Microsoft.Extensions.Options;


namespace LHA.Notification.Infrastructure.Channels.WebPush;

public sealed class VapidKeyProvider
{
    private readonly WebPushProviderSettings _config;

    public VapidKeyProvider(IOptions<WebPushProviderSettings> config)
    {
        _config = config.Value;
    }

    public string Subject => _config.Subject;
    public string PublicKey => _config.PublicKey;
    public string PrivateKey => _config.PrivateKey;
}

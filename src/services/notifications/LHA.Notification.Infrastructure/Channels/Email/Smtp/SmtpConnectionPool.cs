using MailKit.Net.Smtp;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;


namespace LHA.Notification.Infrastructure.Channels.Email.Smtp;

internal sealed class SmtpConnectionPool : IDisposable
{
    private readonly ObjectPool<SmtpClient> _pool;

    public SmtpConnectionPool(IOptions<SmtpProviderSettings> config)
    {
        SmtpProviderSettings settings = config.Value;
        var policy = new SmtpClientPooledObjectPolicy(settings);
        _pool = new DefaultObjectPool<SmtpClient>(policy, settings.PoolSize);
    }

    public SmtpClient Get() => _pool.Get();

    public void Return(SmtpClient client) => _pool.Return(client);

    public void Dispose()
    {
    }

    private sealed class SmtpClientPooledObjectPolicy(SmtpProviderSettings settings) : PooledObjectPolicy<SmtpClient>
    {
        public override SmtpClient Create()
        {
            var client = new SmtpClient();
            client.Connect(settings.Host, settings.Port, settings.UseSsl);
            if (!string.IsNullOrEmpty(settings.Username))
            {
                client.Authenticate(settings.Username, settings.Password ?? string.Empty);
            }
            return client;
        }

        public override bool Return(SmtpClient obj)
        {
            if (!obj.IsConnected)
                return false;
            return true;
        }
    }
}

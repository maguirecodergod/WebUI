using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace LHA.MessageBroker.RabbitMQ;

/// <summary>
/// Manages the lifecycle of RabbitMQ connections and channels.
/// Provides thread-safe, lazy-initialized connection and a shared publish channel.
/// Consumer channels are created per-subscription (best practice for RabbitMQ).
/// </summary>
public sealed class RabbitMqConnectionManager : IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqConnectionManager> _logger;

    private IConnection? _connection;
    private IChannel? _publishChannel;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private readonly SemaphoreSlim _channelLock = new(1, 1);
    private bool _disposed;

    public RabbitMqConnectionManager(
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqConnectionManager> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Gets or creates the shared RabbitMQ connection with retry logic.
    /// Thread-safe and lazy-initialized.
    /// </summary>
    public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_connection is { IsOpen: true })
            return _connection;

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_connection is { IsOpen: true })
                return _connection;

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                AutomaticRecoveryEnabled = _options.AutomaticRecoveryEnabled,
                NetworkRecoveryInterval = TimeSpan.FromMilliseconds(_options.NetworkRecoveryIntervalMs),
                ClientProvidedName = _options.ConnectionName ?? $"lha-{Environment.MachineName}-{Environment.ProcessId}"
            };

            for (var attempt = 0; attempt <= _options.MaxConnectionRetries; attempt++)
            {
                try
                {
                    _connection = await factory.CreateConnectionAsync(cancellationToken);

                    _logger.LogInformation(
                        "RabbitMQ connection established to {Host}:{Port}/{VHost}",
                        _options.HostName, _options.Port, _options.VirtualHost);

                    return _connection;
                }
                catch (Exception ex) when (attempt < _options.MaxConnectionRetries)
                {
                    _logger.LogWarning(ex,
                        "RabbitMQ connection attempt {Attempt}/{MaxRetries} failed, retrying...",
                        attempt + 1, _options.MaxConnectionRetries);

                    var delay = TimeSpan.FromMilliseconds(
                        _options.ConnectionRetryDelayMs * Math.Pow(2, attempt));
                    await Task.Delay(delay, cancellationToken);
                }
            }

            throw new InvalidOperationException(
                $"Failed to connect to RabbitMQ at {_options.HostName}:{_options.Port} " +
                $"after {_options.MaxConnectionRetries + 1} attempts");
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    /// <summary>
    /// Gets or creates the shared publish channel.
    /// This channel is reused across all publish calls (thread-safe in RabbitMQ.Client 7.x).
    /// </summary>
    public async Task<IChannel> GetPublishChannelAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_publishChannel is { IsOpen: true })
            return _publishChannel;

        await _channelLock.WaitAsync(cancellationToken);
        try
        {
            if (_publishChannel is { IsOpen: true })
                return _publishChannel;

            var connection = await GetConnectionAsync(cancellationToken);
            _publishChannel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            _logger.LogDebug("RabbitMQ publish channel created");
            return _publishChannel;
        }
        finally
        {
            _channelLock.Release();
        }
    }

    /// <summary>
    /// Creates a new consumer channel. Each consumer should have its own channel
    /// (RabbitMQ best practice for concurrent consumers).
    /// </summary>
    public async Task<IChannel> CreateConsumerChannelAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var connection = await GetConnectionAsync(cancellationToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        _logger.LogDebug("RabbitMQ consumer channel created");
        return channel;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        if (_publishChannel is not null)
        {
            if (_publishChannel.IsOpen)
                await _publishChannel.CloseAsync();
            _publishChannel.Dispose();
        }

        if (_connection is not null)
        {
            if (_connection.IsOpen)
                await _connection.CloseAsync();
            _connection.Dispose();
        }

        _connectionLock.Dispose();
        _channelLock.Dispose();

        _logger.LogInformation("RabbitMQ connection manager disposed");
    }
}

using LHA.Shared.Contracts.Security;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LHA.AspNetCore.Security;

public sealed class RedisSecurityVersionManager : ISecurityVersionManager, IDisposable
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<RedisSecurityVersionManager> _logger;
    private readonly SecurityVersioningOptions _options;
    private readonly Lazy<IConnectionMultiplexer> _connection;
    private readonly object _circuitLock = new();
    private int _consecutiveFailures;
    private DateTimeOffset? _circuitOpenedUntil;

    public RedisSecurityVersionManager(
        IMemoryCache memoryCache,
        IOptions<SecurityVersioningOptions> options,
        ILogger<RedisSecurityVersionManager> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _options = options.Value;
        _connection = new Lazy<IConnectionMultiplexer>(CreateConnection);
    }

    public async Task<long> GetMaxVersionAsync(string userId, IReadOnlyCollection<string> roles, CancellationToken cancellationToken = default)
    {
        var keys = BuildKeys(userId, roles);
        if (keys.Count == 0)
        {
            return 0;
        }

        var values = new Dictionary<string, long>(StringComparer.Ordinal);
        var misses = new List<string>();

        foreach (var key in keys)
        {
            if (_memoryCache.TryGetValue<long>(key, out var cached))
            {
                values[key] = cached;
            }
            else
            {
                misses.Add(key);
            }
        }

        if (misses.Count > 0)
        {
            var fetched = await TryGetManyAsync(misses, cancellationToken);
            foreach (var (key, value) in fetched)
            {
                values[key] = value;
                _memoryCache.Set(key, value, _options.L1CacheTtl);
            }

            foreach (var key in misses)
            {
                if (!values.ContainsKey(key))
                {
                    values[key] = 0;
                    _memoryCache.Set(key, 0L, _options.L1CacheTtl);
                }
            }
        }

        return values.Values.Count == 0 ? 0 : values.Values.Max();
    }

    public async Task<bool> IsIssuedAtValidAsync(long issuedAtUnixSeconds, string userId, IReadOnlyCollection<string> roles, CancellationToken cancellationToken = default)
    {
        var maxVersion = await GetMaxVersionAsync(userId, roles, cancellationToken);
        return issuedAtUnixSeconds >= maxVersion;
    }

    public Task<long> BumpUserAsync(string userId, CancellationToken cancellationToken = default)
        => BumpAsync(UserKey(userId), cancellationToken);

    public Task<long> BumpRoleAsync(string roleName, CancellationToken cancellationToken = default)
        => BumpAsync(RoleKey(roleName), cancellationToken);

    private async Task<long> BumpAsync(string key, CancellationToken cancellationToken)
    {
        var version = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _memoryCache.Set(key, version, _options.L1CacheTtl);

        if (IsCircuitOpen())
        {
            _logger.LogWarning("Security version Redis circuit is open. Version bump for {Key} kept in L1 only.", key);
            return version;
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _connection.Value.GetDatabase().StringSetAsync(key, version);
            MarkSuccess();
        }
        catch (Exception ex) when (ex is RedisException or RedisConnectionException or TimeoutException)
        {
            MarkFailure(ex);
        }

        return version;
    }

    private async Task<Dictionary<string, long>> TryGetManyAsync(List<string> keys, CancellationToken cancellationToken)
    {
        var result = new Dictionary<string, long>(StringComparer.Ordinal);
        if (IsCircuitOpen())
        {
            return result;
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            RedisKey[] redisKeys = keys.Select(k => new RedisKey(k)).ToArray();;
            var values = await _connection.Value.GetDatabase().StringGetAsync(redisKeys);

            for (var i = 0; i < keys.Count; i++)
            {
                if (values[i].HasValue && long.TryParse(values[i].ToString(), out var version))
                {
                    result[keys[i]] = version;
                }
            }

            MarkSuccess();
        }
        catch (Exception ex) when (ex is RedisException or RedisConnectionException or TimeoutException)
        {
            MarkFailure(ex);
        }

        return result;
    }

    private IConnectionMultiplexer CreateConnection()
    {
        var configuration = ConfigurationOptions.Parse(_options.RedisConfiguration);
        configuration.AbortOnConnectFail = false;
        configuration.ConnectRetry = 1;
        configuration.ConnectTimeout = 500;
        configuration.SyncTimeout = 500;
        return ConnectionMultiplexer.Connect(configuration);
    }

    private bool IsCircuitOpen()
    {
        lock (_circuitLock)
        {
            if (_circuitOpenedUntil is null)
            {
                return false;
            }

            if (_circuitOpenedUntil > DateTimeOffset.UtcNow)
            {
                return true;
            }

            _circuitOpenedUntil = null;
            _consecutiveFailures = 0;
            return false;
        }
    }

    private void MarkSuccess()
    {
        lock (_circuitLock)
        {
            _consecutiveFailures = 0;
            _circuitOpenedUntil = null;
        }
    }

    private void MarkFailure(Exception exception)
    {
        lock (_circuitLock)
        {
            _consecutiveFailures++;
            if (_consecutiveFailures >= _options.CircuitBreakerFailureThreshold)
            {
                _circuitOpenedUntil = DateTimeOffset.UtcNow.Add(_options.CircuitBreakerBreakDuration);
            }
        }

        _logger.LogWarning(exception, "Security version Redis check failed. Requests fail open while Redis is unavailable.");
    }

    private static List<string> BuildKeys(string userId, IReadOnlyCollection<string> roles)
    {
        var keys = new List<string>();
        if (!string.IsNullOrWhiteSpace(userId))
        {
            keys.Add(UserKey(userId));
        }

        foreach (var role in roles.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            keys.Add(RoleKey(role));
        }

        return keys;
    }

    private static string UserKey(string userId) => $"SecVer:User:{userId}";

    private static string RoleKey(string roleName) => $"SecVer:Role:{roleName}";

    public void Dispose()
    {
        if (_connection.IsValueCreated)
        {
            _connection.Value.Dispose();
        }
    }
}


using System.Diagnostics;
using LHA.DistributedLocking;
using Medallion.Threading;
using MedallionLocking = Medallion.Threading;

namespace LHA.DistributedLocking.Medallion;

/// <summary>
/// Verifies the Medallion distributed lock provider is reachable and operational
/// by attempting to acquire and release a short-lived probe lock.
/// </summary>
public sealed class MedallionDistributedLockHealthCheck : IDistributedLockHealthCheck
{
    private const string ProbeLockName = "__lha_distributed_lock_health_probe__";

    private readonly MedallionLocking.IDistributedLockProvider _provider;
    private readonly IDistributedLockKeyNormalizer _keyNormalizer;

    public MedallionDistributedLockHealthCheck(
        MedallionLocking.IDistributedLockProvider provider,
        IDistributedLockKeyNormalizer keyNormalizer)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(keyNormalizer);
        _provider = provider;
        _keyNormalizer = keyNormalizer;
    }

    /// <inheritdoc />
    public async Task<DistributedLockHealthResult> CheckAsync(CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            var key = _keyNormalizer.NormalizeKey(ProbeLockName);
            await using var handle = await _provider.TryAcquireLockAsync(
                key, TimeSpan.Zero, cancellationToken);

            sw.Stop();

            return handle is not null
                ? new DistributedLockHealthResult(true, "Lock provider is healthy.", sw.Elapsed.TotalMilliseconds)
                : new DistributedLockHealthResult(true, "Lock provider reachable but probe lock is held.", sw.Elapsed.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new DistributedLockHealthResult(false, $"Lock provider error: {ex.Message}", sw.Elapsed.TotalMilliseconds);
        }
    }
}

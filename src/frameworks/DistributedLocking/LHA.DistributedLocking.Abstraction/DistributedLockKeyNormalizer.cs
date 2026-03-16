using Microsoft.Extensions.Options;

namespace LHA.DistributedLocking;

/// <summary>
/// Default key normalizer that prepends the configured <see cref="DistributedLockOptions.KeyPrefix"/>.
/// </summary>
public sealed class DistributedLockKeyNormalizer : IDistributedLockKeyNormalizer
{
    private readonly DistributedLockOptions _options;

    public DistributedLockKeyNormalizer(IOptions<DistributedLockOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options.Value;
    }

    /// <inheritdoc />
    public string NormalizeKey(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return string.IsNullOrEmpty(_options.KeyPrefix)
            ? name
            : string.Concat(_options.KeyPrefix, name);
    }
}

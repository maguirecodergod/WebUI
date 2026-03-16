namespace LHA.DistributedLocking;

/// <summary>
/// Normalizes lock keys before they are used to acquire a lock.
/// Useful for adding application-level prefixes or tenant isolation.
/// </summary>
public interface IDistributedLockKeyNormalizer
{
    /// <summary>
    /// Returns the normalized form of the given lock <paramref name="name"/>.
    /// </summary>
    string NormalizeKey(string name);
}

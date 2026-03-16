namespace LHA.Caching;

/// <summary>
/// Serializes and deserializes cache items to and from byte arrays
/// for storage in <see cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache"/>.
/// </summary>
public interface IDistributedCacheSerializer
{
    /// <summary>
    /// Serializes <paramref name="value"/> to a byte array.
    /// </summary>
    byte[] Serialize<T>(T value);

    /// <summary>
    /// Deserializes a byte array to an instance of <typeparamref name="T"/>.
    /// </summary>
    T Deserialize<T>(byte[] bytes);
}

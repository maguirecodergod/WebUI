using System.Reflection;

namespace LHA.Caching;

/// <summary>
/// Specifies a custom cache name for a cache item type.
/// When not applied, the full type name (minus a trailing <c>CacheItem</c> suffix) is used.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
public sealed class CacheNameAttribute : Attribute
{
    /// <summary>
    /// The custom cache name.
    /// </summary>
    public string Name { get; }

    public CacheNameAttribute(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }

    /// <summary>
    /// Resolves the cache name for <typeparamref name="TCacheItem"/>.
    /// </summary>
    public static string GetCacheName<TCacheItem>() => GetCacheName(typeof(TCacheItem));

    /// <summary>
    /// Resolves the cache name for the given <paramref name="cacheItemType"/>.
    /// Uses <see cref="CacheNameAttribute.Name"/> when present,
    /// otherwise falls back to the full type name stripped of a trailing <c>CacheItem</c> suffix.
    /// </summary>
    public static string GetCacheName(Type cacheItemType)
    {
        ArgumentNullException.ThrowIfNull(cacheItemType);

        var attribute = cacheItemType.GetCustomAttribute<CacheNameAttribute>();
        if (attribute is not null)
        {
            return attribute.Name;
        }

        var fullName = cacheItemType.FullName!;
        return fullName.EndsWith("CacheItem", StringComparison.Ordinal)
            ? fullName[..^"CacheItem".Length]
            : fullName;
    }
}

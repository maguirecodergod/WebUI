using System.Collections.Concurrent;
using LHA.Localization.Abstraction;
using Microsoft.Extensions.Localization;

namespace LHA.Localization;

/// <summary>
/// Factory for creating <see cref="LHAStringLocalizer"/> instances.
/// Implements both <see cref="IStringLocalizerFactory"/> (for standard DI)
/// and <see cref="ILHAStringLocalizerFactory"/> (for resource-type-aware creation).
/// Localizer instances are cached per resource type.
/// </summary>
public sealed class LHAStringLocalizerFactory : ILHAStringLocalizerFactory
{
    private readonly LocalizationResourceManager _resourceManager;
    private readonly ConcurrentDictionary<Type, IStringLocalizer> _localizerCache = new();

    public LHAStringLocalizerFactory(LocalizationResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
    }

    /// <inheritdoc />
    public IStringLocalizer CreateLocalizer<TResource>() where TResource : ILocalizationResource
    {
        return CreateLocalizer(typeof(TResource));
    }

    /// <inheritdoc />
    public IStringLocalizer CreateLocalizer(Type resourceType)
    {
        return _localizerCache.GetOrAdd(resourceType,
            type => new LHAStringLocalizer(type, _resourceManager));
    }

    /// <inheritdoc />
    /// <remarks>
    /// Standard IStringLocalizerFactory implementation. Uses the baseName and location
    /// to attempt to find a matching registered resource type.
    /// Falls back to creating a localizer for the base name as type.
    /// </remarks>
    public IStringLocalizer Create(string baseName, string location)
    {
        // Try to resolve as a registered resource type
        var type = Type.GetType($"{baseName}, {location}");
        if (type is not null)
            return CreateLocalizer(type);

        // Fallback: use baseName as a key and create a non-typed localizer
        // This supports standard IStringLocalizer usage without resource types
        return _localizerCache.GetOrAdd(
            typeof(string), // placeholder type for non-resource-based usage
            _ => new LHAStringLocalizer(typeof(object), _resourceManager));
    }

    /// <inheritdoc />
    public IStringLocalizer Create(Type resourceType)
    {
        return CreateLocalizer(resourceType);
    }
}

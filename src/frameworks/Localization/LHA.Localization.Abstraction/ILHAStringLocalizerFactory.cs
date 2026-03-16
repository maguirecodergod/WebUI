using Microsoft.Extensions.Localization;

namespace LHA.Localization.Abstraction;

/// <summary>
/// Factory for creating LHA string localizers for a specific resource type.
/// Extends the standard <see cref="IStringLocalizerFactory"/> with
/// resource-type-aware creation.
/// </summary>
public interface ILHAStringLocalizerFactory : IStringLocalizerFactory
{
    /// <summary>
    /// Creates a string localizer for the specified resource type.
    /// </summary>
    /// <typeparam name="TResource">The localization resource marker type.</typeparam>
    /// <returns>A string localizer instance.</returns>
    IStringLocalizer CreateLocalizer<TResource>() where TResource : ILocalizationResource;

    /// <summary>
    /// Creates a string localizer for the specified resource type.
    /// </summary>
    /// <param name="resourceType">The localization resource marker type.</param>
    /// <returns>A string localizer instance.</returns>
    IStringLocalizer CreateLocalizer(Type resourceType);
}

using System.Globalization;
using LHA.Localization.Abstraction;
using Microsoft.Extensions.Localization;

namespace LHA.Localization;

/// <summary>
/// LHA implementation of <see cref="IStringLocalizer"/> that resolves
/// localization strings from the <see cref="LocalizationResourceManager"/>.
/// Supports culture fallback: specific culture → neutral culture → default culture.
/// </summary>
public sealed class LHAStringLocalizer : IStringLocalizer
{
    private readonly Type _resourceType;
    private readonly LocalizationResourceManager _resourceManager;

    public LHAStringLocalizer(Type resourceType, LocalizationResourceManager resourceManager)
    {
        _resourceType = resourceType;
        _resourceManager = resourceManager;
    }

    /// <inheritdoc />
    public LocalizedString this[string name]
    {
        get
        {
            var value = GetString(name);
            return new LocalizedString(name, value ?? name, resourceNotFound: value is null);
        }
    }

    /// <inheritdoc />
    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var value = GetString(name);
            var formatted = value is not null
                ? string.Format(CultureInfo.CurrentUICulture, value, arguments)
                : name;
            return new LocalizedString(name, formatted, resourceNotFound: value is null);
        }
    }

    /// <inheritdoc />
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var cultureName = CultureInfo.CurrentUICulture.Name;
        var strings = _resourceManager.GetStrings(_resourceType, cultureName);

        if (includeParentCultures)
        {
            var descriptor = _resourceManager.GetDescriptor(_resourceType);
            var defaultCulture = descriptor?.DefaultCulture ?? "en";

            // Merge default culture strings (as fallback)
            if (!string.Equals(cultureName, defaultCulture, StringComparison.OrdinalIgnoreCase))
            {
                var defaultStrings = _resourceManager.GetStrings(_resourceType, defaultCulture);
                var merged = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                foreach (var kvp in defaultStrings)
                    merged[kvp.Key] = kvp.Value;
                foreach (var kvp in strings)
                    merged[kvp.Key] = kvp.Value;

                return merged.Select(kvp =>
                    new LocalizedString(kvp.Key, kvp.Value, resourceNotFound: false));
            }
        }

        return strings.Select(kvp =>
            new LocalizedString(kvp.Key, kvp.Value, resourceNotFound: false));
    }

    /// <summary>
    /// Resolves a string with culture fallback chain:
    /// CurrentUICulture → neutral culture → default culture.
    /// </summary>
    private string? GetString(string name)
    {
        var culture = CultureInfo.CurrentUICulture;
        var descriptor = _resourceManager.GetDescriptor(_resourceType);
        var defaultCulture = descriptor?.DefaultCulture ?? "en";

        // Try specific culture (e.g. "en-US")
        var strings = _resourceManager.GetStrings(_resourceType, culture.Name);
        if (strings.TryGetValue(name, out var value))
            return value;

        // Try neutral culture (e.g. "en")
        if (culture.Parent is { Name.Length: > 0 } parent)
        {
            strings = _resourceManager.GetStrings(_resourceType, parent.Name);
            if (strings.TryGetValue(name, out value))
                return value;
        }

        // Fallback to default culture
        if (!string.Equals(culture.Name, defaultCulture, StringComparison.OrdinalIgnoreCase))
        {
            strings = _resourceManager.GetStrings(_resourceType, defaultCulture);
            if (strings.TryGetValue(name, out value))
                return value;
        }

        return null;
    }
}

/// <summary>
/// Generic typed string localizer that wraps <see cref="LHAStringLocalizer"/>
/// for a specific resource type.
/// </summary>
/// <typeparam name="TResource">The localization resource marker type.</typeparam>
public sealed class LHAStringLocalizer<TResource> : IStringLocalizer<TResource>
{
    private readonly IStringLocalizer _innerLocalizer;

    public LHAStringLocalizer(ILHAStringLocalizerFactory factory)
    {
        _innerLocalizer = factory.CreateLocalizer(typeof(TResource));
    }

    /// <inheritdoc />
    public LocalizedString this[string name] => _innerLocalizer[name];

    /// <inheritdoc />
    public LocalizedString this[string name, params object[] arguments] => _innerLocalizer[name, arguments];

    /// <inheritdoc />
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        _innerLocalizer.GetAllStrings(includeParentCultures);
}

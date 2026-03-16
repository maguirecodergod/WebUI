namespace LHA.Localization.Abstraction;

/// <summary>
/// Options for configuring the LHA localization system.
/// Registered via the Options pattern in DI.
/// </summary>
public sealed class LocalizationResourceOptions
{
    /// <summary>
    /// Registered localization resource descriptors.
    /// </summary>
    public IList<LocalizationResourceDescriptor> Resources { get; } = [];

    /// <summary>
    /// Global default culture used when a resource does not specify one.
    /// </summary>
    public string DefaultCulture { get; set; } = "en";

    /// <summary>
    /// Supported cultures across the application.
    /// </summary>
    public IList<string> SupportedCultures { get; } = ["en", "vi"];

    /// <summary>
    /// Registers a resource type with automatic metadata extraction.
    /// </summary>
    /// <typeparam name="TResource">The resource marker type.</typeparam>
    /// <param name="configure">Optional callback to further configure the descriptor.</param>
    /// <returns>This options instance for chaining.</returns>
    public LocalizationResourceOptions AddResource<TResource>(
        Action<LocalizationResourceDescriptor>? configure = null)
        where TResource : ILocalizationResource
    {
        return AddResource(typeof(TResource), configure);
    }

    /// <summary>
    /// Registers a resource type with automatic metadata extraction.
    /// </summary>
    /// <param name="resourceType">The resource marker type.</param>
    /// <param name="configure">Optional callback to further configure the descriptor.</param>
    /// <returns>This options instance for chaining.</returns>
    public LocalizationResourceOptions AddResource(
        Type resourceType,
        Action<LocalizationResourceDescriptor>? configure = null)
    {
        var attribute = resourceType
            .GetCustomAttributes(typeof(LocalizationResourceAttribute), false)
            .OfType<LocalizationResourceAttribute>()
            .FirstOrDefault();

        var resourceNamespace = resourceType.Namespace ?? resourceType.Assembly.GetName().Name ?? "";
        var resourceName = attribute?.ResourceName ?? resourceType.Name;

        // When an explicit ResourceName is set, the convention is that JSON files
        // live in a subfolder named after that value, e.g.:
        //   Localization/Identity/en.json  →  embedded as  …Localization.Identity.en.json
        // When ResourceName comes from the type name (no attribute), files sit
        // directly in the namespace folder, e.g.:
        //   Localization/en.json  →  embedded as  …Localization.en.json
        var resourceBasePath = attribute?.ResourceName is not null
            ? $"{resourceNamespace}.{attribute.ResourceName}"
            : resourceNamespace;

        var descriptor = new LocalizationResourceDescriptor
        {
            ResourceType = resourceType,
            ResourceBasePath = resourceBasePath,
            ResourceName = resourceName,
            DefaultCulture = attribute?.DefaultCulture ?? DefaultCulture
        };

        configure?.Invoke(descriptor);
        Resources.Add(descriptor);
        return this;
    }
}

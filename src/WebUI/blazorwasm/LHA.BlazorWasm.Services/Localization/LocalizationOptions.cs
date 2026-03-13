namespace LHA.BlazorWasm.Services.Localization;

/// <summary>
/// Configuration options for the Localization Service.
/// </summary>
public class LocalizationOptions
{
    /// <summary>
    /// The default culture if none is previously selected.
    /// </summary>
    public LanguageCode DefaultCulture { get; set; } = LanguageCode.EN;

    /// <summary>
    /// The supported cultures in the application.
    /// </summary>
    public List<LanguageCode> SupportedCultures { get; set; } = new() { LanguageCode.EN, LanguageCode.VI };

    /// <summary>
    /// The local storage key used to persist the selected language. Our ILocalStorageService 
    /// will automatically prepend the "app:" prefix to this internally.
    /// </summary>
    public string LocalStorageKey { get; set; } = "language";

    /// <summary>
    /// A list of URLs/paths template pointing to localization JSON resources to load.
    /// Uses '{0}' as a placeholder for the culture name (e.g., 'en').
    /// </summary>
    public List<string> ResourcePaths { get; set; } = new()
    {
        "/localization/{0}.json"
    };
}

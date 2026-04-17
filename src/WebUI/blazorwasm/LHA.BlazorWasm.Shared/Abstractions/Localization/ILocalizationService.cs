using LHA.BlazorWasm.Shared.Models.Localization;

namespace LHA.BlazorWasm.Shared.Abstractions.Localization;

/// <summary>
/// Interface for the enterprise Localization service.
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Gets the current state container holding the active Culture and Translations dictionary.
    /// </summary>
    LocalizationState State { get; }

    /// <summary>
    /// Event triggered when the language has been successfully switched and loaded.
    /// Used by Blazor components to trigger StateHasChanged() natively.
    /// </summary>
    event Action? OnLanguageChanged;

    /// <summary>
    /// Gets the translated string for the specified dot-notation key (e.g., "Common.Save").
    /// </summary>
    /// <param name="key">The translation key.</param>
    /// <returns>The localized string, or the key itself if no translation is found.</returns>
    string L(string key);

    /// <summary>
    /// Gets the translated string for the specified dot-notation key (e.g., "Common.Save").
    /// </summary>
    /// <param name="key">The translation key.</param>
    /// <param name="args">The arguments to replace in the translation string.</param>
    /// <returns>The localized string, or the key itself if no translation is found.</returns>
    string L(string key, params object[] args);

    /// <summary>
    /// Changes the active language dynamically.
    /// Connects to LocalStorage, fetches necessary module JSON resources, and triggers UI updates.
    /// </summary>
    /// <param name="culture">The culture code (e.g., "en", "vi").</param>
    Task SetLanguageAsync(string culture);

    /// <summary>
    /// Initializes the service by reading the persisted language (or falling back to default)
    /// and asynchronously loading the required resources. Best called during app startup.
    /// </summary>
    Task InitializeAsync();
}

using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using LHA.BlazorWasm.Services.Storage;

namespace LHA.BlazorWasm.Services.Localization;

/// <summary>
/// Implementation of the Localization Service responsible for pulling from wwwroot, 
/// merging module keys natively, and storing in local storage cache.
/// </summary>
internal sealed class LocalizationService : ILocalizationService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly LocalizationOptions _options;

    // In-memory cache holding Culture -> (Module -> Translations dict) to avoid re-fetching JSONs.
    private readonly Dictionary<string, Dictionary<string, string>> _translationCache = new();

    public LocalizationState State { get; } = new();

    public event Action? OnLanguageChanged;

    public LocalizationService(
        HttpClient httpClient,
        ILocalStorageService localStorage,
        IOptions<LocalizationOptions> options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        _options = options?.Value ?? new LocalizationOptions();
    }

    public string L(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return string.Empty;

        return State.Translations.TryGetValue(key, out var value) ? value : key;
    }

    public async Task InitializeAsync()
    {
        // 1. Check local storage for existing preference
        var savedCulture = await _localStorage.GetAsync<string>(_options.LocalStorageKey);

        // 2. Fallback to default if not present or unsupported natively
        var targetCulture = !string.IsNullOrEmpty(savedCulture) &&
                            LanguageProvider.GetOptions(_options.SupportedCultures).Any(x => x.Culture == savedCulture)
            ? savedCulture
            : LanguageProvider.GetOption(_options.DefaultCulture).Culture;

        // 3. Load the language
        await SetLanguageAsync(targetCulture);
    }

    public async Task SetLanguageAsync(string culture)
    {
        if (!LanguageProvider.GetOptions(_options.SupportedCultures).Any(x => x.Culture == culture))
        {
            culture = LanguageProvider.GetOption(_options.DefaultCulture).Culture;
        }

        // Fast path if already cached natively
        if (_translationCache.TryGetValue(culture, out var cachedTranslations))
        {
            ApplyLanguageState(culture, cachedTranslations);
            await PersistCultureAsync(culture);
            return;
        }

        // Fetch mechanism
        var compiledTranslations = new Dictionary<string, string>();

        foreach (var pathTemplate in _options.ResourcePaths)
        {
            var url = string.Format(pathTemplate, culture);

            try
            {
                var jsonDocument = await _httpClient.GetFromJsonAsync<JsonDocument>(url);
                if (jsonDocument != null)
                {
                    FlattenJson(jsonDocument.RootElement, string.Empty, compiledTranslations);
                }
            }
            catch (Exception)
            {
                // Gracefully ignore missing module files so other modules can still load
            }
        }

        _translationCache[culture] = compiledTranslations;

        ApplyLanguageState(culture, compiledTranslations);
        await PersistCultureAsync(culture);
    }

    private void ApplyLanguageState(string culture, Dictionary<string, string> translations)
    {
        State.CurrentCulture = culture;
        State.Translations = translations;

        var cultureInfo = (System.Globalization.CultureInfo)new System.Globalization.CultureInfo(culture).Clone();

        // Ensure AM/PM designators are mapped from translations if available
        if (translations.TryGetValue("Time.AM", out var am)) cultureInfo.DateTimeFormat.AMDesignator = am;
        if (translations.TryGetValue("Time.PM", out var pm)) cultureInfo.DateTimeFormat.PMDesignator = pm;

        System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        System.Globalization.CultureInfo.CurrentCulture = cultureInfo;
        System.Globalization.CultureInfo.CurrentUICulture = cultureInfo;

        OnLanguageChanged?.Invoke();
    }

    private async Task PersistCultureAsync(string culture)
    {
        await _localStorage.SetAsync(_options.LocalStorageKey, culture);
    }

    /// <summary>
    /// Recursively flattens a JSON structure into dot-notation strings.
    /// Ex: { "User": { "Title": "Hello" } } -> "User.Title": "Hello".
    /// </summary>
    private void FlattenJson(JsonElement element, string prefix, Dictionary<string, string> dictionary)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                var key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";
                FlattenJson(property.Value, key, dictionary);
            }
        }
        else if (element.ValueKind == JsonValueKind.String)
        {
            dictionary[prefix] = element.GetString() ?? string.Empty;
        }
    }
}

namespace LHA.Localization.Abstraction;

/// <summary>
/// Marker interface for localization resource types.
/// Each module should define a class implementing this interface
/// to serve as a resource identifier for its localization files.
/// </summary>
/// <example>
/// <code>
/// [LocalizationResource(DefaultCulture = "en")]
/// public class MultiTenancyResource : ILocalizationResource { }
/// </code>
/// </example>
public interface ILocalizationResource;

namespace LHA.Localization.Abstraction;

/// <summary>
/// Configures a localization resource type with metadata such as
/// default culture and resource name.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class LocalizationResourceAttribute : Attribute
{
    /// <summary>
    /// The default/fallback culture for this resource (e.g. "en", "vi").
    /// Defaults to "en".
    /// </summary>
    public string DefaultCulture { get; set; } = "en";

    /// <summary>
    /// Optional custom resource name. If not set, the type name is used.
    /// </summary>
    public string? ResourceName { get; set; }
}

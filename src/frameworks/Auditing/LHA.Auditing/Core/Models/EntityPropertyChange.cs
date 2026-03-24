namespace LHA.Auditing;

/// <summary>
/// Tracks a single property change within an <see cref="EntityChangeEntry"/>.
/// </summary>
public sealed class EntityPropertyChange
{
    /// <summary>Maximum length of <see cref="PropertyName"/>. Value: 96.</summary>
    public const int MaxPropertyNameLength = 96;

    /// <summary>Maximum length of <see cref="NewValue"/> and <see cref="OriginalValue"/>. Value: 512.</summary>
    public const int MaxValueLength = 512;

    /// <summary>Maximum length of <see cref="PropertyTypeFullName"/>. Value: 512.</summary>
    public const int MaxPropertyTypeFullNameLength = 512;

    /// <summary>Name of the changed property.</summary>
    public required string PropertyName { get; init; }

    /// <summary>CLR full type name of the property.</summary>
    public required string PropertyTypeFullName { get; init; }

    /// <summary>Serialized original value (<c>null</c> for Created changes).</summary>
    public string? OriginalValue { get; set; }

    /// <summary>Serialized new value (<c>null</c> for Deleted changes).</summary>
    public string? NewValue { get; set; }
}

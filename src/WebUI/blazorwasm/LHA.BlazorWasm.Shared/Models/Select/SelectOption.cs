namespace LHA.BlazorWasm.Shared.Models.Select;

/// <summary>
/// Represents a generic option in the Select component.
/// </summary>
/// <typeparam name="T">The type of the underlying value.</typeparam>
public class SelectOption<T>
{
    /// <summary>
    /// The unique value identifying the option.
    /// </summary>
    public T Value { get; set; } = default!;

    /// <summary>
    /// The display label for the option.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Optional icon for the option (e.g., Bootstrap Icons class).
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Optional description for the option.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether the option is disabled and cannot be selected.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Optional group name for categorizing options.
    /// </summary>
    public string? Group { get; set; }
}

namespace LHA.BlazorWasm.Components.Select;

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
    /// Whether the option is disabled and cannot be selected.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Optional group name for categorizing options.
    /// </summary>
    public string? Group { get; set; }
}

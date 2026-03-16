namespace LHA.BlazorWasm.Components.Buttons;

/// <summary>
/// Specifies the type of the button.
/// </summary>
public enum CButtonType
{
    /// <summary>
    /// A clickable button.
    /// </summary>
    Button,

    /// <summary>
    /// A button that submits form data to a server.
    /// </summary>
    Submit,

    /// <summary>
    /// A button that resets form data to its initial values.
    /// </summary>
    Reset
}

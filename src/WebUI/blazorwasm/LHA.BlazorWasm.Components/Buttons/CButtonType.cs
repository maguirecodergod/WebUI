namespace LHA.BlazorWasm.Components.Buttons;

/// <summary>
/// Specifies the type of the button.
/// </summary>
public enum CButtonType
{
    /// <summary>
    /// 0 - Button: A clickable button.
    /// </summary>
    Button,

    /// <summary>
    /// 1 - Submit: A button that submits form data to a server.
    /// </summary>
    Submit,

    /// <summary>
    /// 2 - Reset: A button that resets form data to its initial values.
    /// </summary>
    Reset
}

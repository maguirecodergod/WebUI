using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;

namespace LHA.BlazorWasm.Components.Buttons;

/// <summary>
/// A reusable enterprise-grade Button component.
/// 
/// Example usage:
/// 
/// <Button Style="ButtonStyle.Primary"
///         Size="ButtonSize.Medium"
///         OnClick="SaveAsync">
///     Save
/// </Button>
/// </summary>
public partial class Button : ComponentBase
{
    /// <summary>
    /// Gets or sets the text content of the button.
    /// Overridden by ChildContent if provided.
    /// </summary>
    [Parameter] public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the icon class (e.g., 'fas fa-save' or a custom icon class).
    /// </summary>
    [Parameter] public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the position of the icon.
    /// </summary>
    [Parameter] public ButtonIconPosition IconPosition { get; set; } = ButtonIconPosition.Left;

    /// <summary>
    /// Gets or sets the style variant of the button.
    /// </summary>
    [Parameter] public ButtonStyle Style { get; set; } = ButtonStyle.Primary;

    /// <summary>
    /// Gets or sets the size of the button.
    /// </summary>
    [Parameter] public ButtonSize Size { get; set; } = ButtonSize.Medium;

    /// <summary>
    /// Gets or sets a value indicating whether the button is in a loading state.
    /// When true, the button is disabled, the text is hidden, and a spinner is shown.
    /// </summary>
    [Parameter] public bool IsLoading { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the button is explicitly disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the button should span the full width of its container.
    /// </summary>
    [Parameter] public bool FullWidth { get; set; }

    /// <summary>
    /// Gets or sets additional custom CSS classes for the button.
    /// </summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the button is clicked.
    /// Supports async event handlers automatically.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Gets or sets the child content to render inside the button.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Determines if the button should be disabled in the DOM.
    /// </summary>
    private bool IsDisabled => Disabled || IsLoading;

    /// <summary>
    /// Helper method to construct the CSS class string for the button based on its state and properties.
    /// </summary>
    /// <returns>A space-separated string of CSS classes.</returns>
    private string GetCssClass()
    {
        var classes = new List<string> { "base-btn" };

        classes.Add($"btn-{Style.ToString().ToLowerInvariant()}");
        classes.Add($"btn-{Size.ToString().ToLowerInvariant()}");

        if (FullWidth)
        {
            classes.Add("btn-full-width");
        }

        if (IsLoading)
        {
            classes.Add("btn-loading");
        }

        if (IsDisabled)
        {
            classes.Add("btn-disabled");
        }

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(" ", classes);
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LHA.BlazorWasm.Components.Switch;

/// <summary>
/// A toggle switch component similar to iOS/Android switches.
/// Supports two-way binding, different sizes, labels, icons, tooltips, and keyboard accessibility.
/// 
/// Example usage:
/// 
/// <Switch @bind-Value="isEnabled" Label="Enable feature" />
/// 
/// <Switch @bind-Value="isEnabled" OffIcon="fas fa-times" OnIcon="fas fa-check" />
/// 
/// <Switch @bind-Value="isEnabled" Tooltip="Toggle feature on/off" />
/// </summary>
public partial class Switch : LHAComponentBase
{
    /// <summary>
    /// Gets or sets the current state of the switch (true = ON, false = OFF).
    /// </summary>
    [Parameter] public bool Value { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the value changes.
    /// Used for two-way binding.
    /// </summary>
    [Parameter] public EventCallback<bool> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the switch is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the optional label text displayed with the switch.
    /// </summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the size of the switch.
    /// </summary>
    [Parameter] public CSwitchSize Size { get; set; } = CSwitchSize.Medium;

    /// <summary>
    /// Gets or sets the position of the label relative to the switch.
    /// </summary>
    [Parameter] public CSwitchLabelPosition LabelPosition { get; set; } = CSwitchLabelPosition.Right;

    /// <summary>
    /// Gets or sets additional custom CSS classes.
    /// </summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the icon class to display when the switch is OFF (false).
    /// Example: "fas fa-times" or "fa-solid fa-power-off"
    /// </summary>
    [Parameter] public string? OffIcon { get; set; }

    /// <summary>
    /// Gets or sets the icon class to display when the switch is ON (true).
    /// Example: "fas fa-check" or "fa-solid fa-power-on"
    /// </summary>
    [Parameter] public string? OnIcon { get; set; }

    /// <summary>
    /// Gets or sets the tooltip text to display when hovering over the switch.
    /// </summary>
    [Parameter] public string? Tooltip { get; set; }

    /// <summary>
    /// Gets or sets whether to show icons inside the switch thumb.
    /// </summary>
    [Parameter] public bool ShowIcons { get; set; }

    /// <summary>
    /// Determines if the switch should show tooltip.
    /// </summary>
    private bool HasTooltip => !string.IsNullOrEmpty(Tooltip);

    /// <summary>
    /// Determines if icons should be displayed inside the switch.
    /// </summary>
    private bool ShouldShowIcons => ShowIcons && (!string.IsNullOrEmpty(OffIcon) || !string.IsNullOrEmpty(OnIcon));

    /// <summary>
    /// Toggles the switch state between on and off.
    /// </summary>
    private async Task Toggle()
    {
        if (Disabled)
        {
            return;
        }

        Value = !Value;
        await ValueChanged.InvokeAsync(Value);
    }

    /// <summary>
    /// Handles keyboard events for accessibility (Space and Enter keys).
    /// </summary>
    /// <param name="args">The keyboard event arguments.</param>
    private async Task HandleKeyDown(KeyboardEventArgs args)
    {
        if (Disabled)
        {
            return;
        }

        if (args.Key == " " || args.Key == "Enter")
        {
            await Toggle();
        }
    }

    /// <summary>
    /// Builds the CSS class string based on component state and parameters.
    /// </summary>
    /// <returns>A space-separated string of CSS classes.</returns>
    private string GetCssClass()
    {
        var classes = new List<string> { "lha-switch" };

        // Add size class
        classes.Add($"size-{Size.ToString().ToLowerInvariant()}");

        // Add checked state class
        if (Value)
        {
            classes.Add("is-checked");
        }

        // Add disabled state class
        if (Disabled)
        {
            classes.Add("is-disabled");
        }

        // Add icon mode class
        if (ShouldShowIcons)
        {
            classes.Add("has-icons");
        }

        // Add custom class if provided
        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(" ", classes);
    }
}

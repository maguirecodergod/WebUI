using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Section;

/// <summary>
/// A collapsible/expandable section container component for grouping UI content.
///
/// Example usage:
///
/// <Section Title="User Information" Collapsible="true">
///     <FormField Label="Name">
///         <InputText @bind-Value="User.Name" />
///     </FormField>
/// </Section>
/// </summary>
public partial class Section : LhaComponentBase
{
    /// <summary>
    /// Gets or sets the title displayed in the section header.
    /// </summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>
    /// Gets or sets an optional icon (emoji or icon class) displayed before the title.
    /// </summary>
    [Parameter] public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the visual variant of the section.
    /// </summary>
    [Parameter] public SectionVariant Variant { get; set; } = SectionVariant.Default;

    /// <summary>
    /// Gets or sets whether the section can be collapsed/expanded by clicking the header.
    /// </summary>
    [Parameter] public bool Collapsible { get; set; }

    /// <summary>
    /// Gets or sets the default expanded state when the component is first rendered.
    /// Only used when <see cref="Expanded"/> is not externally bound.
    /// </summary>
    [Parameter] public bool DefaultExpanded { get; set; } = true;

    /// <summary>
    /// Gets or sets the externally-controlled expanded state.
    /// When set, the component operates in controlled mode.
    /// </summary>
    [Parameter] public bool? Expanded { get; set; }

    /// <summary>
    /// Callback fired when the expanded state changes.
    /// </summary>
    [Parameter] public EventCallback<bool> OnToggle { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes for the root element.
    /// </summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>
    /// Gets or sets inline styles for the root element.
    /// </summary>
    [Parameter] public string? Style { get; set; }

    /// <summary>
    /// Gets or sets the child content rendered inside the section body.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Internal expanded state for uncontrolled mode.
    /// </summary>
    private bool _internalExpanded;

    /// <summary>
    /// Effective expanded state, respecting controlled vs uncontrolled mode.
    /// </summary>
    private bool IsExpanded => Expanded ?? _internalExpanded;

    protected override void OnInitialized()
    {
        _internalExpanded = DefaultExpanded;
    }

    /// <summary>
    /// Handles the header click to toggle the expanded state.
    /// </summary>
    private async Task ToggleAsync()
    {
        if (!Collapsible) return;

        if (Expanded.HasValue)
        {
            // Controlled mode — notify parent
            await OnToggle.InvokeAsync(!Expanded.Value);
        }
        else
        {
            // Uncontrolled mode — manage internally
            _internalExpanded = !_internalExpanded;
            await OnToggle.InvokeAsync(_internalExpanded);
        }
    }

    /// <summary>
    /// Constructs the CSS class string for the root section element.
    /// </summary>
    private string GetCssClass()
    {
        var classes = new List<string> { "section" };

        classes.Add($"section-{Variant.ToString().ToLowerInvariant()}");

        if (IsExpanded)
        {
            classes.Add("section-expanded");
        }

        if (Collapsible)
        {
            classes.Add("section-collapsible");
        }

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(" ", classes);
    }
}

using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Form;

/// <summary>
/// Enterprise-grade form field wrapper component that standardizes layout, labeling,
/// help text, and validation for any form control.
///
/// Example usage:
///
/// <FormField Label="Email" Required="true" Help="We'll never share your email.">
///     <InputText @bind-Value="Model.Email" class="form-control" />
/// </FormField>
/// </summary>
public partial class FormField : ComponentBase
{
    /// <summary>
    /// Gets or sets the label text for the field.
    /// </summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>
    /// Gets or sets whether the field is required. When true, a red asterisk is shown after the label.
    /// </summary>
    [Parameter] public bool Required { get; set; }

    /// <summary>
    /// Gets or sets the help text displayed below the control.
    /// </summary>
    [Parameter] public string? Help { get; set; }

    /// <summary>
    /// Gets or sets the layout mode (Vertical or Horizontal).
    /// </summary>
    [Parameter] public FormFieldLayout Layout { get; set; } = FormFieldLayout.Vertical;

    /// <summary>
    /// Gets or sets additional custom CSS classes.
    /// </summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>
    /// Gets or sets inline styles for the root element.
    /// </summary>
    [Parameter] public string? Style { get; set; }

    /// <summary>
    /// Gets or sets the child content (the actual form control).
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the value expression for validation message binding.
    /// Enables integration with Blazor EditForm's ValidationMessage component.
    /// </summary>
    [Parameter] public Expression<Func<object>>? ValueExpression { get; set; }

    private readonly string _fieldId = $"ff-{Guid.NewGuid():N}";
    private string _helpId => $"{_fieldId}-help";

    /// <summary>
    /// Constructs the CSS class string for the root form-field element.
    /// </summary>
    private string GetCssClass()
    {
        var classes = new List<string> { "form-field" };

        if (Layout == FormFieldLayout.Horizontal)
        {
            classes.Add("form-field-horizontal");
        }

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(" ", classes);
    }
}

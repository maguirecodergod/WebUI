using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.RichTextEditor.Components;

public partial class ToolbarButton : LhaComponentBase
{
    [Parameter] public string? SvgIcon { get; set; }
    [Parameter] public string? Tooltip { get; set; }
    [Parameter] public bool IsActive { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string? CommandName { get; set; }
    [Parameter] public EventCallback OnClick { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private string GetCssClass()
    {
        var classes = "rte-toolbar-btn";
        if (IsActive) classes += " active";
        if (Disabled) classes += " disabled";
        return classes;
    }

    private async Task HandleClick()
    {
        if (!Disabled && OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync();
        }
    }
}

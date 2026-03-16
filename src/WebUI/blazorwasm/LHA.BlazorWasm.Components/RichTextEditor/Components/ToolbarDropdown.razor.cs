using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Components.RichTextEditor.Models;

namespace LHA.BlazorWasm.Components.RichTextEditor.Components;

public partial class ToolbarDropdown : LhaComponentBase
{
    [Parameter] public string? SvgIcon { get; set; }
    [Parameter] public string? Tooltip { get; set; }
    [Parameter] public bool IsActive { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public CDropdownAlignment Alignment { get; set; } = CDropdownAlignment.Left;
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private bool IsOpen { get; set; }

    private string GetButtonCssClass()
    {
        var classes = "rte-toolbar-btn rte-dropdown-btn";
        if (IsActive) classes += " active";
        if (IsOpen) classes += " open";
        if (Disabled) classes += " disabled";
        return classes;
    }

    private void ToggleDropdown()
    {
        if (!Disabled)
        {
            IsOpen = !IsOpen;
        }
    }

    public void Close()
    {
        IsOpen = false;
        StateHasChanged();
    }
}

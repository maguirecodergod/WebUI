using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace LHA.BlazorWasm.Components.Select;

public partial class SelectItem : LhaComponentBase
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public bool IsSelected { get; set; }
    [Parameter] public bool IsFocused { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public EventCallback OnSelect { get; set; }

    private async Task HandleClick()
    {
        if (Disabled) return;
        await OnSelect.InvokeAsync();
    }
}

using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;

namespace LHA.BlazorWasm.Components.Select;

public class SelectState<TValue>
{
    public bool IsOpen { get; set; }
    public string SearchText { get; set; } = string.Empty;
    public int FocusedIndex { get; set; } = -1;
    public bool IsLoading { get; set; }
    
    /// <summary>
    /// Flag to prevent FocusOut from closing the popup prematurely during internal clicks.
    /// </summary>
    public bool PreventFocusClose { get; set; }

    public ItemsProviderDelegate<SelectOption<TValue>>? ItemsProvider { get; set; }
    public event Action? OnOptionsChanged;

    public void NotifyOptionsChanged() => OnOptionsChanged?.Invoke();
}

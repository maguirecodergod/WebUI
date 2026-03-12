using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LHA.BlazorWasm.Components.Pickers.Core;

/// <summary>
/// Abstract base defining robust shared UI parameters ensuring zero-configuration consistency across all Pickers.
/// Integrates pure C# blur mechanics and custom events.
/// </summary>
public abstract class PickerBase<TValue> : ComponentBase
{
    [Parameter] public TValue? Value { get; set; }
    [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }
    
    [Parameter] public string Placeholder { get; set; } = "Select...";
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    
    [Parameter] public DateTime? Min { get; set; }
    [Parameter] public DateTime? Max { get; set; }
    
    [Parameter] public string Format { get; set; } = "yyyy-MM-dd";
    [Parameter] public bool ShowClear { get; set; } = true;
    
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    
    [Parameter] public EventCallback<TValue?> OnChange { get; set; }

    protected PickerState State { get; } = new();

    protected async Task UpdateValueAsync(TValue? newValue)
    {
        Value = newValue;
        await ValueChanged.InvokeAsync(Value);
        if (OnChange.HasDelegate)
        {
            await OnChange.InvokeAsync(Value);
        }
    }

    public void TogglePopup()
    {
        if (Disabled || ReadOnly) return;
        State.IsOpen = !State.IsOpen;
    }

    public void ClosePopup()
    {
        if (State.IsOpen)
        {
            State.IsOpen = false;
        }
    }
    
    protected virtual Task ClearAsync()
    {
        return UpdateValueAsync(default);
    }
    
    /// <summary>
    /// Traps global click-away logic purely using Blazor FocusOut bubbling techniques.
    /// Handled cautiously via tab/blur mechanics rather than global JS interceptors.
    /// </summary>
    protected void OnFocusOut()
    {
        // Delay needed to allow interactive clicks inside the dropdown to process first.
        Task.Delay(150).ContinueWith(_ => 
        {
            InvokeAsync(() => 
            {
                if (State.IsOpen)
                {
                    State.IsOpen = false;
                    StateHasChanged();
                }
            });
        });
    }
}

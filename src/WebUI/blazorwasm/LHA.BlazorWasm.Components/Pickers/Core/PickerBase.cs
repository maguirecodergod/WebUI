using LHA.BlazorWasm.Services.Localization;
using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Pickers.Core;

/// <summary>
/// Abstract base defining robust shared UI parameters ensuring zero-configuration consistency across all Pickers.
/// Integrates pure C# blur mechanics and custom events.
/// </summary>
public abstract class PickerBase<TValue> : ComponentBase, IDisposable
{
    [Inject] protected ILocalizationService LocalizationService { get; set; } = default!;

    [Parameter] public TValue? Value { get; set; }
    [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }

    [Parameter] public string? Placeholder { get; set; }
    protected string EffectivePlaceholder => string.IsNullOrEmpty(Placeholder) 
        ? LocalizationService.L("Common.Select") 
        : Placeholder;

    protected override void OnInitialized()
    {
        LocalizationService.OnLanguageChanged += HandleLanguageChanged;
    }

    private void HandleLanguageChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    public virtual void Dispose()
    {
        LocalizationService.OnLanguageChanged -= HandleLanguageChanged;
    }

    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool ReadOnly { get; set; }

    [Parameter] public DateTime? Min { get; set; }
    [Parameter] public DateTime? Max { get; set; }
    [Parameter] public Func<DateTime, bool>? DisabledDateFunc { get; set; }

    [Parameter] public string Format { get; set; } = string.Empty;
    [Parameter] public bool ShowClear { get; set; } = true;

    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }

    [Parameter] public ValidationStatus ValidationStatus { get; set; } = ValidationStatus.None;
    [Parameter] public string? ValidationMessage { get; set; }

    protected string ValidationClass => ValidationStatus switch
    {
        ValidationStatus.Success => "success",
        ValidationStatus.Warning => "warning",
        ValidationStatus.Error => "error",
        _ => ""
    };

    [Parameter] public EventCallback<TValue?> OnChange { get; set; }
    
    [Parameter] public IPickerValueConverter<TValue>? Converter { get; set; }
    protected IPickerValueConverter<TValue> EffectiveConverter => Converter ?? new DefaultPickerConverter<TValue>();

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
        HandleInternalInteraction(); // Record click time
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

    private DateTime _lastInteractionTime = DateTime.MinValue;

    /// <summary>
    /// Captures mousedown or touchstart events inside the component to prevent FocusOut from closing the popup prematurely.
    /// </summary>
    protected void HandleInternalInteraction()
    {
        _lastInteractionTime = DateTime.Now;
    }

    /// <summary>
    /// Traps global click-away logic purely using Blazor FocusOut bubbling techniques.
    /// Handled cautiously via tab/blur mechanics rather than global JS interceptors.
    /// </summary>
    protected async Task OnFocusOut()
    {
        // Delay needed to allow interactive clicks inside the dropdown to process first.
        // Increased for mobile stability.
        await Task.Delay(300);

        // If an internal interaction happened very recently (including the click that opened it), don't close.
        if ((DateTime.Now - _lastInteractionTime).TotalMilliseconds < 500)
        {
            return;
        }

        if (State.IsOpen)
        {
            State.IsOpen = false;
            StateHasChanged();
        }
    }
}

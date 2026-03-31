using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace LHA.BlazorWasm.Components.Forms;

/// <summary>
/// A helper component that connects FluentValidation to Blazor's EditContext.
/// </summary>
public sealed class FluentValidationValidator : ComponentBase, IDisposable
{
    private ValidationMessageStore? _messageStore;

    [CascadingParameter]
    private EditContext? CurrentEditContext { get; set; }

    [Parameter]
    public IValidator? Validator { get; set; }

    [Inject]
    private LHA.BlazorWasm.Shared.Abstractions.Localization.ILocalizationService Localizer { get; set; } = default!;

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (CurrentEditContext == null)
        {
            throw new InvalidOperationException($"{nameof(FluentValidationValidator)} requires a cascading parameter of type {nameof(EditContext)}.");
        }

        _messageStore = new ValidationMessageStore(CurrentEditContext);
        CurrentEditContext.OnValidationRequested += (s, e) => ValidateModel();
        CurrentEditContext.OnFieldChanged += (s, e) => ValidateField(e.FieldIdentifier);
        Localizer.OnLanguageChanged += HandleLanguageChanged;
    }

    private void HandleLanguageChanged()
    {
        if (CurrentEditContext == null) return;
        
        // Only re-validate if there are already some validation messages (from a previous validation)
        if (CurrentEditContext.GetValidationMessages().Any())
        {
            ValidateModel();
            InvokeAsync(StateHasChanged);
        }
    }

    private void ValidateModel()
    {
        if (CurrentEditContext == null || _messageStore == null) return;

        var validator = GetValidator();
        if (validator == null) return;

        var validationContext = new ValidationContext<object>(CurrentEditContext.Model);
        var result = validator.Validate(validationContext);

        _messageStore.Clear();
        foreach (var error in result.Errors)
        {
            _messageStore.Add(new FieldIdentifier(CurrentEditContext.Model, error.PropertyName), Localizer.L(error.ErrorMessage));
        }

        CurrentEditContext.NotifyValidationStateChanged();
    }

    private void ValidateField(FieldIdentifier fieldIdentifier)
    {
        if (CurrentEditContext == null || _messageStore == null) return;

        var validator = GetValidator();
        if (validator == null) return;

        var validationContext = new ValidationContext<object>(CurrentEditContext.Model, 
            new FluentValidation.Internal.PropertyChain(), 
            new FluentValidation.Internal.MemberNameValidatorSelector(new[] { fieldIdentifier.FieldName }));
        
        var result = validator.Validate(validationContext);

        _messageStore.Clear(fieldIdentifier);
        foreach (var error in result.Errors.Where(e => e.PropertyName == fieldIdentifier.FieldName))
        {
            _messageStore.Add(fieldIdentifier, Localizer.L(error.ErrorMessage));
        }

        CurrentEditContext.NotifyValidationStateChanged();
    }

    private IValidator? GetValidator()
    {
        if (Validator != null) return Validator;

        var modelType = CurrentEditContext?.Model.GetType();
        if (modelType == null) return null;

        var validatorType = typeof(IValidator<>).MakeGenericType(modelType);
        return ServiceProvider.GetService(validatorType) as IValidator;
    }

    public void Dispose()
    {
        if (Localizer != null)
        {
            Localizer.OnLanguageChanged -= HandleLanguageChanged;
        }

        if (CurrentEditContext != null)
        {
            CurrentEditContext.OnValidationRequested -= (s, e) => ValidateModel();
            CurrentEditContext.OnFieldChanged -= (s, e) => ValidateField(e.FieldIdentifier);
        }
    }
}

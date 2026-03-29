using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;

namespace LHA.BlazorWasm.Components.Forms;

/// <summary>
/// A component to be placed inside an <EditForm> to display validation errors
/// returned from the server API calls.
/// </summary>
public class ServerSideValidator : ComponentBase
{
    private ValidationMessageStore? _messageStore;

    [CascadingParameter]
    private EditContext? CurrentEditContext { get; set; }

    protected override void OnInitialized()
    {
        if (CurrentEditContext == null)
        {
            throw new InvalidOperationException(
                $"{nameof(ServerSideValidator)} requires a cascading " +
                $"parameter of type {nameof(EditContext)}. " +
                $"For example, you can use {nameof(ServerSideValidator)} " +
                $"inside an {nameof(EditForm)}.");
        }

        _messageStore = new ValidationMessageStore(CurrentEditContext);

        // Clear errors whenever a field changes or the form submits validly
        CurrentEditContext.OnValidationRequested += (s, e) => _messageStore.Clear();
        CurrentEditContext.OnFieldChanged += (s, e) => _messageStore.Clear(e.FieldIdentifier);
    }

    public void DisplayErrors(IDictionary<string, string[]> errors)
    {
        _messageStore?.Clear();
        
        if (errors == null) return;

        foreach (var err in errors)
        {
            var fieldName = err.Key;
            
            // Map JSON property names (camelCase) to Model property names (PascalCase) if needed,
            // but usually Blazor validation handles case well if properties match exactly.
            if (!string.IsNullOrEmpty(fieldName) && char.IsLower(fieldName[0]))
            {
                fieldName = char.ToUpperInvariant(fieldName[0]) + fieldName.Substring(1);
            }

            var fieldIdentifier = new FieldIdentifier(CurrentEditContext!.Model, fieldName);

            foreach (var message in err.Value)
            {
                _messageStore?.Add(fieldIdentifier, message);
            }
        }

        CurrentEditContext?.NotifyValidationStateChanged();
    }
    
    public void DisplayErrors(IDictionary<string, List<string>> errors)
    {
        _messageStore?.Clear();
        
        if (errors == null) return;

        foreach (var err in errors)
        {
            var fieldIdentifier = new FieldIdentifier(CurrentEditContext!.Model, err.Key);

            foreach (var message in err.Value)
            {
                _messageStore?.Add(fieldIdentifier, message);
            }
        }

        CurrentEditContext?.NotifyValidationStateChanged();
    }

    public void ClearErrors()
    {
        _messageStore?.Clear();
        CurrentEditContext?.NotifyValidationStateChanged();
    }
}
